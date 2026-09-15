# syntax=docker/dockerfile:1.6
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim

ARG NODE_MAJOR=22
ARG DEV_USER=dev
ARG DEV_UID=1000
ARG DEV_GID=1000

ENV DEBIAN_FRONTEND=noninteractive \
    LANG=en_US.UTF-8 \
    LC_ALL=en_US.UTF-8 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

# ---- Base packages ----------------------------------------------------------
# sqlite3 CLI: the app stores its data in a SQLite file at the repo root.
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
        ca-certificates curl gnupg xz-utils unzip \
        git build-essential \
        ripgrep fd-find jq less vim sqlite3 \
        openssh-server sudo \
        locales tini \
 && sed -i '/en_US.UTF-8/s/^# //' /etc/locale.gen && locale-gen \
 && rm -rf /var/lib/apt/lists/*

# fd is installed as `fdfind` on Debian; provide the conventional `fd` alias.
RUN ln -s /usr/bin/fdfind /usr/local/bin/fd

# ---- Node.js (NodeSource) + Claude Code -------------------------------------
RUN curl -fsSL "https://deb.nodesource.com/setup_${NODE_MAJOR}.x" | bash - \
 && apt-get install -y --no-install-recommends nodejs \
 && rm -rf /var/lib/apt/lists/* \
 && npm install -g @anthropic-ai/claude-code \
 && npm cache clean --force

# ---- Dev user ---------------------------------------------------------------
RUN groupadd --gid "${DEV_GID}" "${DEV_USER}" \
 && useradd  --uid "${DEV_UID}" --gid "${DEV_GID}" --create-home --shell /bin/bash "${DEV_USER}" \
 && echo "${DEV_USER} ALL=(ALL) NOPASSWD:ALL" > "/etc/sudoers.d/${DEV_USER}" \
 && chmod 0440 "/etc/sudoers.d/${DEV_USER}" \
 && install -d -m 0700 -o "${DEV_USER}" -g "${DEV_USER}" "/home/${DEV_USER}/.ssh" \
 && install -d -m 0755 -o "${DEV_USER}" -g "${DEV_USER}" \
        "/home/${DEV_USER}/.cache" "/home/${DEV_USER}/.local" \
        "/home/${DEV_USER}/.config" "/home/${DEV_USER}/.claude" \
        "/home/${DEV_USER}/.nuget" "/home/${DEV_USER}/.dotnet"

# ---- Env for sshd/PAM login shells ------------------------------------------
# Docker ENV does not propagate into ssh sessions; PAM reads /etc/environment.
# PATH must keep the system dirs or ssh logins lose dotnet/node/git/etc.
# ~/.dotnet/tools is where `dotnet tool install -g dotnet-ef` lands.
RUN printf '%s\n' \
        'PATH="/home/dev/.dotnet/tools:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"' \
        'DOTNET_ROOT="/usr/share/dotnet"' \
        'DOTNET_CLI_TELEMETRY_OPTOUT="1"' \
        'DOTNET_NOLOGO="1"' \
        'LANG="en_US.UTF-8"' \
        >> /etc/environment

# ---- SSH server -------------------------------------------------------------
# Hardened sshd config (pubkey-only, no root, dev user only).
COPY --chmod=0644 .devcontainer/sshd_config /etc/ssh/sshd_config.d/00-dev.conf
RUN sed -i 's/\r$//' /etc/ssh/sshd_config.d/00-dev.conf

# Privilege separation directory required by sshd.
RUN install -d -m 0755 /var/run/sshd

# Entrypoint: install authorized_keys, generate host keys, exec sshd.
COPY --chmod=0755 .devcontainer/entrypoint.sh /usr/local/bin/entrypoint.sh
RUN sed -i 's/\r$//' /usr/local/bin/entrypoint.sh

# The repo is bind-mounted with host ownership; without this git refuses to
# operate ("dubious ownership") for both SSH and VS Code sessions.
RUN git config --system --add safe.directory /workspaces/campaigen

# ---- Git auth: fine-grained per-repo PAT over https -------------------------
# The credential store sits on the ssh-config named volume so the token
# survives container recreates. NEVER an SSH key to GitHub in here — SSH keys
# cannot be scoped per-repo, and this container must not reach beyond its own
# repo (see DEVCONTAINER_TEMPLATE.md).
RUN git config --system credential.helper 'store --file /home/dev/.ssh/git-credentials' \
 && git config --system user.name "Anton Tegnelöv" \
 && git config --system user.email "anton.tegnelov@gmail.com" \
 # The Windows host checked the worktree out with core.autocrlf=true (CRLF
 # on disk, LF in the index). The container shares that worktree via the
 # bind mount, so it must apply the same conversion or every text file
 # shows as modified. .gitattributes still forces LF for .devcontainer/**.
 && git config --system core.autocrlf true

WORKDIR /workspaces/campaigen
EXPOSE 22

# tini is provided by docker-compose's `init: true`, which runs it as PID 1.
# Don't wrap with another tini here or you get nested-tini warnings and
# broken signal handling.
ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]

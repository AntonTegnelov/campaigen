# campaigen dev container

An SSH-reachable Linux dev box for campaigen, following the house pattern in
`E:\Programmering\Code\DEVCONTAINER_TEMPLATE.md` — **this one uses host
port 2236** (see the template's port table for the full allocation).

## What's inside

| Tool | Version / notes |
|---|---|
| .NET SDK | 8.0 (`mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim` base) |
| Node.js | 22 (NodeSource) — only for Claude Code |
| sqlite3 | CLI, for poking at `campaigen_data.db` |
| Claude Code | latest, installed globally via npm |
| sshd | hardened: pubkey-only, no root, `dev` user only, host port **2236** |

The repo is bind-mounted at `/workspaces/campaigen`, so edits sync both ways.
Any gitignored `.env` arrives via the mount — it is never baked into the image
(`.dockerignore` keeps it out of the build context).

**Named volumes** (survive recreates, including Zed's dev-container flow):
`~/.nuget` (package cache), `~/.dotnet` (global tools such as `dotnet-ef`),
`~/.claude`, `~/.ssh` (incl. the PAT store), and the sshd host keys.

**`bin/` and `obj/` are shared with Windows through the bind mount.** The
restore outputs in `obj/` embed absolute paths, so after switching sides
(host <-> container) the first `dotnet build` re-runs restore automatically —
expected, not an error. Run `dotnet restore` explicitly if a tool complains.

## One-time setup

Make sure `%USERPROFILE%\.ssh\authorized_keys` on the host contains your
public key:

```
type %USERPROFILE%\.ssh\id_ed25519.pub >> %USERPROFILE%\.ssh\authorized_keys
```

The entrypoint installs this file into the container on every start, so key
changes only need `docker compose restart` — no rebuild.

## Build and start

```
docker compose -f .devcontainer/docker-compose.yml up -d --build
```

The container auto-restarts with Docker Desktop (`restart: unless-stopped`).

Stop: `docker compose -f .devcontainer/docker-compose.yml down`
(add `-v` to also wipe the NuGet cache, dotnet tools, the Claude login, the
PAT store, and ssh host keys).

## Connect

Add to `~/.ssh/config` on the host:

```
Host campaigen-dev
    HostName localhost
    Port 2236
    User dev
```

Then `ssh campaigen-dev`, or point Claude Code / Cursor / JetBrains Gateway at
it. Zed: `zed ssh://dev@localhost:2236/workspaces/campaigen`, or "Reopen in
Dev Container" (recreates once on first attach — state is on volumes, so it
survives). VS Code: "Dev Containers: Reopen in Container", which runs
`dotnet restore` via `postCreateCommand`.

## First-login project setup (SSH users)

```bash
cd /workspaces/campaigen
dotnet restore Campaigen.sln
dotnet build Campaigen.sln
dotnet test Campaigen.sln
dotnet tool install -g dotnet-ef        # once; lands on the dotnet-home volume
dotnet ef database update --project src/Campaigen.Core.Infrastructure --startup-project src/Campaigen.CLI
dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- --help
```

## Git identity / push

Commit identity and the credential helper are baked into the image's system
gitconfig — nothing to configure. Pushing uses a **fine-grained per-repo
PAT** over https, **never an SSH key**: GitHub SSH keys can't be scoped to
one repo, and this container must not reach beyond its own repo (see
DEVCONTAINER_TEMPLATE.md).

One-time, after minting the PAT (GitHub -> Settings -> Developer settings ->
Fine-grained tokens -> Repository access: only `AntonTegnelov/campaigen` ->
Permissions: Contents = Read and write):

```bash
printf 'https://AntonTegnelov:%s@github.com\n' '<the PAT>' > ~/.ssh/git-credentials
chmod 600 ~/.ssh/git-credentials
```

(Or just `git push` once and answer the prompt — username `AntonTegnelov`,
password = the PAT; the credential helper writes the same file.) The store
lives on the `ssh-config` named volume, so the login survives container
recreates and rebuilds.

## Claude Code

`claude` is preinstalled. Log in once (`claude` -> follow the OAuth flow);
credentials live on the `claude-config` named volume and survive rebuilds.

## Troubleshooting

- **`Permission denied (publickey)`** — check `%USERPROFILE%\.ssh\authorized_keys`
  contains your pubkey, then `docker compose -f .devcontainer/docker-compose.yml restart`.
- **Host key changed after `down -v`** — the host-key volume was wiped; run
  `ssh-keygen -R "[localhost]:2236"` on the host and reconnect.
- **NU1xxx / "assets file doesn't have a target"** — the `obj/` state was
  written by the other side; run `dotnet restore Campaigen.sln`.
- **`dotnet-ef` not found** — `dotnet tool install -g dotnet-ef` (PATH already
  includes `~/.dotnet/tools` for ssh logins).

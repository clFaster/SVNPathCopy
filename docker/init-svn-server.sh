#!/bin/sh
set -eu

REPO_NAME="${REPO_NAME:-testrepo}"
SVN_USER="${SVN_USER:-svnadmin}"
SVN_PASS="${SVN_PASS:-svnadmin}"

REPO_ROOT="/var/opt/svn"
REPO_DIR="$REPO_ROOT/$REPO_NAME"

if [ ! -d "$REPO_DIR" ]; then
  svnadmin create "$REPO_DIR"
fi

CONF_DIR="$REPO_DIR/conf"

cat > "$CONF_DIR/svnserve.conf" << 'EOF'
[general]
anon-access = none
auth-access = write
password-db = passwd
EOF

cat > "$CONF_DIR/passwd" << EOF
[users]
$SVN_USER = $SVN_PASS
EOF

exec svnserve --daemon --foreground --root "$REPO_ROOT" --listen-host 0.0.0.0 --listen-port 3690

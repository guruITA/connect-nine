#!/bin/sh
# Runs on the test server as the user deploy, started by the job deploy-test over ssh (task 3.9, #267).
# The job has just unpacked the new version into /srv/connect-test/incoming: app/, efbundle and
# docker-compose.test.yml. /srv/connect-test/.env stays where it is and is never printed.
set -eu

cd /srv/connect-test
compose="docker compose -f docker-compose.test.yml"

mv incoming/docker-compose.test.yml docker-compose.test.yml

# The order of #267: stop the server, apply the migrations, start the server.
# On the very first deploy there is no server yet, and stop does nothing.
$compose stop server

rm -rf app efbundle
mv incoming/app app
mv incoming/efbundle efbundle
rmdir incoming

$compose up -d --wait database
$compose run --rm migrations
# A new container every deploy: the stopped one still points at the app/ folder that was just replaced.
$compose up -d --force-recreate server

echo "The new version runs. The job now asks /api/health."

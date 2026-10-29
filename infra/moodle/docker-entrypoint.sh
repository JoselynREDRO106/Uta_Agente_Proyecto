#!/bin/sh
set -eu

: "${MOODLE_WWWROOT:?Set MOODLE_WWWROOT in the root .env file}"
: "${MOODLE_ADMIN_USER:?Set MOODLE_ADMIN_USER in the root .env file}"
: "${MOODLE_ADMIN_PASSWORD:?Set MOODLE_ADMIN_PASSWORD in the root .env file}"
: "${MOODLE_ADMIN_EMAIL:?Set MOODLE_ADMIN_EMAIL in the root .env file}"
: "${MOODLE_DB_HOST:?Set MOODLE_DB_HOST in the Compose environment}"
: "${MOODLE_DB_NAME:?Set MOODLE_DB_NAME in the root .env file}"
: "${MOODLE_DB_USER:?Set MOODLE_DB_USER in the root .env file}"
: "${MOODLE_DB_PASSWORD:?Set MOODLE_DB_PASSWORD in the root .env file}"

mkdir -p /var/www/moodledata
chown www-data:www-data /var/www/moodledata
ln -sfn /var/www/moodledata/config.php /var/www/html/moodle/config.php

if [ ! -f /var/www/moodledata/config.php ]; then
    attempt=0
    until php -r '
        $connection = @new mysqli(
            getenv("MOODLE_DB_HOST"),
            getenv("MOODLE_DB_USER"),
            getenv("MOODLE_DB_PASSWORD"),
            getenv("MOODLE_DB_NAME")
        );
        exit($connection->connect_errno ? 1 : 0);
    '; do
        attempt=$((attempt + 1))
        if [ "$attempt" -ge 60 ]; then
            echo "MariaDB did not become available after 120 seconds." >&2
            exit 1
        fi
        sleep 2
    done

    su -s /bin/sh www-data -c \
        'php /var/www/html/moodle/admin/cli/install.php \
        --lang=en \
        --wwwroot="$MOODLE_WWWROOT" \
        --dataroot=/var/www/moodledata \
        --dbtype=mariadb \
        --dbhost="$MOODLE_DB_HOST" \
        --dbname="$MOODLE_DB_NAME" \
        --dbuser="$MOODLE_DB_USER" \
        --dbpass="$MOODLE_DB_PASSWORD" \
        --prefix=mdl_ \
        --fullname="EduMonitor AI - Laboratorio Sprint 1" \
        --shortname=EduMonitor-S1 \
        --adminuser="$MOODLE_ADMIN_USER" \
        --adminpass="$MOODLE_ADMIN_PASSWORD" \
        --adminemail="$MOODLE_ADMIN_EMAIL" \
        --non-interactive \
        --agree-license'
fi

exec apache2-foreground

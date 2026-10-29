FROM php:8.3-apache-bookworm

ARG MOODLE_VERSION=4.5.15

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        unzip \
        libcurl4-openssl-dev \
        libfreetype6-dev \
        libicu-dev \
        libjpeg62-turbo-dev \
        libonig-dev \
        libpng-dev \
        libxml2-dev \
        libxslt1-dev \
        libzip-dev \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install -j"$(nproc)" curl exif gd intl mbstring mysqli opcache soap zip \
    && rm -rf /var/lib/apt/lists/*

RUN curl --fail --location --retry 3 \
        "https://download.moodle.org/download.php/direct/stable405/moodle-${MOODLE_VERSION}.zip" \
        --output /tmp/moodle.zip \
    && unzip -q /tmp/moodle.zip -d /var/www/html \
    && rm /tmp/moodle.zip \
    && chown -R www-data:www-data /var/www/html/moodle

COPY apache-moodle.conf /etc/apache2/sites-available/000-default.conf
COPY moodle.ini /usr/local/etc/php/conf.d/moodle.ini
COPY docker-entrypoint.sh /usr/local/bin/edumonitor-moodle-entrypoint

RUN chmod +x /usr/local/bin/edumonitor-moodle-entrypoint \
    && mkdir -p /var/www/moodledata \
    && chown www-data:www-data /var/www/moodledata

WORKDIR /var/www/html/moodle
EXPOSE 80
ENTRYPOINT ["edumonitor-moodle-entrypoint"]

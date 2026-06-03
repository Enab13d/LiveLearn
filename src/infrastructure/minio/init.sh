#!/bin/sh

MINIO_URL=$MINIO_ENDPOINT_URL
ACCESS_KEY=$MINIO_ROOT_USER
SECRET_KEY=$MINIO_ROOT_PASSWORD

echo "Setting up MinIO Client alias..."
/usr/bin/mc alias set localminio "$MINIO_URL" "$ACCESS_KEY" "$SECRET_KEY"

echo "Creating buckets..."
# The --ignore-existing flag keeps the script idempotent if run repeatedly
/usr/bin/mc mb localminio/videos-raw --ignore-existing
/usr/bin/mc mb localminio/videos-hls --ignore-existing
/usr/bin/mc mb localminio/certificates --ignore-existing
/usr/bin/mc mb localminio/media --ignore-existing

# Optional: Make a bucket public for downloads/assets if required
# /usr/bin/mc anonymous set download localminio/videos-raw

echo "MinIO initialization complete!"
exit 0

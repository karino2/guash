#!/usr/bin/env bash

export GUASH_DIR=$(mktemp -d)
guash_readtext "Hello Title"
RES=$(guash_doquery)

echo "$(date):$RES"

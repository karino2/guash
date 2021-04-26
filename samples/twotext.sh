#!/usr/bin/env guash

guash_readtext "First Title"
guash_readtext "Second Title"
guash_doquery RES1 RES2

echo "First=${RES1}, Second=${RES2}"

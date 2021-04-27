#!/usr/bin/env guash

guash_readtext "First Title"
guash_readtext "Second Title"

RES=($(guash_doquery))

echo "First=${RES[0]}, Second=${RES[1]}"

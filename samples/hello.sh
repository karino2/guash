#!/usr/bin/env guash

guash_readtext "Hello Title"
guash_doquery RES

echo `date` ":$RES"

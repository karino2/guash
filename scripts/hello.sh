#!/usr/bin/env guash

guash_readtext 1 "Hello Title"
guash_doquery RES

echo `date` ":$RES"

#!/bin/bash

OPTS="-ruv --exclude=*.meta --exclude=.*"

rsync $OPTS --dry-run ./Assets/Framework/ ~/Dropbox/Code/Unity/Framework/Assets/Framework/

read -p "Are you sure? " -n 1 -r
echo    # (optional) move to a new line
if [[ $REPLY =~ ^[Yy]$ ]]
then
	rsync $OPTS ./Assets/Framework/ ~/Dropbox/Code/Unity/Framework/Assets/Framework/
fi

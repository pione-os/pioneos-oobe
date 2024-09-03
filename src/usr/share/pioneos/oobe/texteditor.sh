#!/bin/sh
apt-get install -y "$1"
apt-get remove -y "$2"

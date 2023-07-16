#!/bin/bash
ITCHIO_USER="camperotactico"
VERSION="./version.txt"
BUILDS_DIR="./builds"
GAME_NAME="excelente-servicio"


for platform in "windows" "linux" "mac" "html5"
do
    BINARY_DIR=$BUILDS_DIR/$platform/
    if [ -d "$BINARY_DIR" ]
    then
        if [ "$(ls -A $BINARY_DIR)" ]; then
        echo "Publishing build for $platform"
        butler push $BINARY_DIR $ITCHIO_USER/$GAME_NAME:$platform --ignore "*DoNotShip*" --userversion-file $VERSION
        else
        echo "$BINARY_DIR is empty. Nothing to publish."
        fi
    else
        echo "Directory $BINARY_DIR not found."
    fi
done
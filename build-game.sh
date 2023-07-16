#!/bin/bash
UNITY_VERSION="2022.3.3f1"
PROJECT_DIR="."
BUILD_LOGS=$PROJECT_DIR/"Logs/builds.log"
BUILDS_DIR=$PROJECT_DIR/"builds"

echo "Deleting previous build files"
rm -rf $BUILDS_DIR/*
rm  $BUILD_LOGS


UNITY_EXEC_PATH=""
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    UNITY_EXEC_PATH="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    UNITY_EXEC_PATH="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity"
fi

echo "Starting Build Process"
if [[ -n "$UNITY_EXEC_PATH" ]]; then
    $UNITY_EXEC_PATH -quit -batchmode -projectPath $PROJECT_DIR -logFile $BUILD_LOGS  -executeMethod BinaryBuilder.PerformAllBuilds
else
    echo "Invalid OS type or unity executable path:"
    echo "OS_TYPE = $OSTYPE"
    echo "UNITY_EXEC_PATH = $UNITY_EXEC_PATH"
fi
echo "Build Process completed. Check the log for errors ($BUILD_LOGS)"
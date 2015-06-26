#XADB Type Checking Script
#version:1.0
#by Team XTek xdavidwu
if [ -d "$1" ] ; then
  echo "folder"
elif [ -f "$1" ] ; then
  echo "file"
else
 echo "error"
fi
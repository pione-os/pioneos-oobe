#!/bin/sh
echo "Windows とのデュアルブートを最適化しています。"
hwclock --verbose --systohc --localtime 
cp /etc/pioneos/oobe/Assets/ReboottoWindows.desktop /usr/share/applications/ReboottoWindows.desktop
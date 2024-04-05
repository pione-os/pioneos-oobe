#!/bin/sh
echo "PioneOS のセットアップを完了しています2"
rm '~/.config/autostart/PioneOS Setup.desktop'
pkill xfconfd
cp /etc/pioneos/xfce-perchannel-xml/xfce4-desktop.xml ~/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /etc/pioneos/xfce-perchannel-xml/xfce4-panel.xml ~/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml


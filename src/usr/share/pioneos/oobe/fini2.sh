#!/bin/sh
echo "PioneOS のセットアップを完了しています2"
rm '~/.config/autostart/PioneOS Setup.desktop'
pkill xfconfd
cp /usr/share/pioneos/xfce-perchannel-xml/xfce4-desktop.xml ~/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /usr/share/pioneos/xfce-perchannel-xml/xfce4-panel.xml ~/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml


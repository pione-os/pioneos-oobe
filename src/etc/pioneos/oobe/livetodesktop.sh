#!/bin/sh
pkill xfconfd
cp /etc/pioneos/xfce-perchannel-xml/xfce4-desktop.xml /home/demo/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /etc/pioneos/xfce-perchannel-xml/xfce4-panel.xml /home/demo/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml
cp /etc/pioneos/xfce-perchannel-xml/xfce4-desktop.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /etc/pioneos/xfce-perchannel-xml/xfce4-panel.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml
rm /etc/pioneos/oobe/oobe
rm /usr/bin/startoobe
systemctl restart lightdm.service && pkill xfce4


#!/bin/sh
echo "PioneOS のセットアップを完了しています"
rm -rf /etc/calamares
rm '/etc/skel/.config/autostart/PioneOS Setup.desktop'
cp /usr/share/pioneos/xfce-perchannel-xml/xfce4-desktop.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /usr/share/pioneos/xfce-perchannel-xml/xfce4-panel.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml
apt-get -y autopurge pioneos-oobe calamares *linux-image-6.1.0*
apt-get clean
rm -f /etc/sudoers.d/pioneos
reboot

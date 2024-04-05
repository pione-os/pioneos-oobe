#!/bin/sh
echo "PioneOS のセットアップを完了しています"
rm -rf /etc/calamares
rm -f /usr/share/applications/install-pioneos.desktop
rm '/etc/skel/.config/autostart/PioneOS Setup.desktop'
cp /etc/pioneos/xfce-perchannel-xml/xfce4-desktop.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-desktop.xml
cp /etc/pioneos/xfce-perchannel-xml/xfce4-panel.xml /etc/skel/.config/xfce4/xfconf/xfce-perchannel-xml/xfce4-panel.xml
apt-get -y remove pioneos-oobe
apt-get -y remove calamares
apt-get -y autopurge *linux-image-6.1.0*
apt-get -y update
reboot

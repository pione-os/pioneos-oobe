#!/bin/sh
apt-get remove -y "$2"
# 引数の説明：$1はインストールするもの、$2はアンインストールするもの、$3はインストール方法、$4はダウンロードする証明書やパッケージ、$5は追加するリポジトリ $6は管理名
howto=$3
if [ "${howto}" = "apt-asc" ]; then
    curl "$4" | gpg --dearmor > "$6".gpg
    install -o root -g root -m 644 "$6".gpg /etc/apt/trusted.gpg.d/
    echo "$5" > /etc/apt/sources.list.d/"$6".list
    rm "$6".gpg
elif [ "${howto}" = "apt-keyrings" ]; then
    curl -fsSL "$4" | gpg --dearmor -o /usr/share/keyrings/"$6".gpg
    echo "$5" > /etc/apt/sources.list.d/"$6".list
elif [ "${howto}" = "apt-key" ]; then
    echo "$5" > /etc/apt/sources.list.d/"$6".list
    wget -O - "$4" | apt-key add -
elif [ "${howto}" = "dpkg" ]; then
    wget "$4"
    dpkg -i *.deb
    rm *.deb
    exit 0
fi
apt-get update
apt-get install -y "$1"
exit 0
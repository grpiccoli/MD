﻿cd /media/guillermo/WD3DNAND-SSD-1TB/
git clone https://github.com/grpltda/ConsultaMD.git
cd /media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/
git pull
cd ConsultaMD/
dotnet ef database drop
dotnet ef database update
#libman restore
wget https://unpkg.com/jquery.mousewheel@3.1.9/jquery.mousewheel.js -O wwwroot/lib/jquery.mousewheel/jquery.mousewheel.js
wget https://unpkg.com/jquery-validation-unobtrusive@3.2.11/dist/jquery.validate.unobtrusive.min.js -O wwwroot/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js
online_md5="$(curl -sL https://unpkg.com/jquery.mousewheel@3.1.9/jquery.mousewheel.js | md5sum | cut -d ' ' -f 1)"
local_md5="$(md5sum "wwwroot/lib/jquery.mousewheel/jquery.mousewheel.js" | cut -d ' ' -f 1)"
if [ "$online_md5" == "$local_md5" ]; then
    echo "hurray, they are equal!"
fi
online_md5="$(curl -sL https://unpkg.com/jquery-validation-unobtrusive@3.2.11/dist/jquery.validate.unobtrusive.min.js | md5sum | cut -d ' ' -f 1)"
local_md5="$(md5sum "wwwroot/lib/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js" | cut -d ' ' -f 1)"
if [ "$online_md5" == "$local_md5" ]; then
    echo "hurray, they are equal!"
fi
npm i
./node_modules/cldr-data-downloader/bin/download.sh -f -i http://www.unicode.org/Public/cldr/26/json.zip -o ./wwwroot/lib/cldr-data
dotnet run
dotnet publish -r linux-x64 -c Release
systemctl stop kestrel-consultamd.service;rm -r ../../webapps/consultamd/;mkdir -p ../../webapps/consultamd/;rsync -auv bin/Release/netcoreapp2.2/linux-x64/publish/* ../../webapps/consultamd/;systemctl start kestrel-consultamd.service

#REQUIREMENTS RUBY
sudo apt install ruby-full ruby-dev g++ make build-essential zlibc zlib1g zlib1g-dev
git clone https://github.com/sagmor/sii_chile.git
sudo gem install sii_chile
ruby test.rb

#REQUIREMENTS WEBPAY NODEJS
sudo apt install python2

#PHP
sudo apt install curl php-cli php-mbstring git unzip
php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');"
php -r "if (hash_file('sha384', 'composer-setup.php') === 'a5c698ffe4b8e849a443b120cd5ba38043260d5c4023dbf93e1558871f1f07f58274fc6f4c93bcfd858c6bd0775cd8d1') { echo 'Installer verified'; } else { echo 'Installer corrupt'; unlink('composer-setup.php'); } echo PHP_EOL;"
php composer-setup.php --install-dir=/usr/local/bin --filename=composer
php -r "unlink('composer-setup.php');"
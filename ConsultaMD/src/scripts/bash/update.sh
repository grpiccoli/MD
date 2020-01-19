#ONE TIME DOTNET
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo add-apt-repository universe
sudo apt update
sudo apt install apt-transport-https
sudo apt update
#sudo apt purge aspnetcore-runtime-2.2 dotnet-runtime-2.2 dotnet-hostfxr-2.2 dotnet-host dotnet-runtime-deps-2.2 dotnet-sdk-2.2
sudo apt install aspnetcore-runtime-2.2=2.2.5-1 dotnet-runtime-2.2=2.2.5-1 dotnet-hostfxr-2.2=2.2.5-1 dotnet-host=2.2.5-1 dotnet-runtime-deps-2.2=2.2.5-1 dotnet-sdk-2.2=2.2.300-1

#ONE TIME MSSQL
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/msodbcsql17/
#https://packages.microsoft.com/ubuntu/16.04/mssql-server-2017/pool/main/m/mssql-server/
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/mssql-tools/
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list)"
sudo apt update
#sudo apt purge msodbcsql17 mssql-server mssql-tools
sudo apt install msodbcsql17=17.3.1.1-1 mssql-server=14.0.3192.2-2 mssql-tools=17.3.0.1-1
sudo /opt/mssql/bin/mssql-conf setup
systemctl status mssql-server --no-pager

#ONE TIME MSSQL TOOLS
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list
sudo apt-get update
sudo apt-get install mssql-tools unixodbc-dev
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
sqlcmd -S localhost -U SA -P 34erdfERDF

#ONE TIME INSTALL LIBMAN
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
#ONE TIME INSTALL NODEJS
curl -sL https://deb.nodesource.com/setup_12.x | sudo -E bash -
sudo apt install -y nodejs
#ONE TIME CHROMIUM
sudo apt install chromium-browser

#cd /media/guillermo/WD3DNAND-SSD-1TB/
git config --global credential.helper store
git clone https://github.com/grpltda/ConsultaMD.git
#XtxGx2B6yjxEmwg
cd ConsultaMD

git reset --hard HEAD
git pull

cd ConsultaMD
dotnet ef database drop
dotnet ef database update
#libman restore
wget https://unpkg.com/jquery.mousewheel@3.1.9/jquery.mousewheel.js -O wwwroot/lib/jquery.mousewheel/jquery.mousewheel.js
wget https://unpkg.com/jquery-hammerjs@2.0.0/jquery.hammer.js -O wwwroot/lib/jquery-hammerjs/jquery.hammer.js
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
systemctl stop kestrel-consultamd.service;
rm -r ../../webapps/consultamd/;
mkdir -p ../../webapps/consultamd/;
rsync -auv bin/Release/netcoreapp2.2/linux-x64/publish/* ~/webapps/consultamd/;
npm --prefix ~/webapps/consultamd/ install ~/webapps/consultamd/
systemctl start kestrel-consultamd.service

#config kestrel
echo <<< EOL
[Unit]
Description=ConsultaMD

[Service]
WorkingDirectory=~/webapps/consultamd
ExecStart=/usr/bin/dotnet ConsultaMD.dll
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-consultamd
User=guillermo
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL >> /etc/systemd/system/kestrel-consultamd.service

sudo vim /etc/systemd/system/kestrel-consultamd.service
systemctl daemon-reload

#ONE TIME MS CONFIGURATION
#1)INSTALL YARN
curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list
sudo apt update
sudo apt install yarn
#2)MOBISCROLL
sudo npm i -g @mobiscroll/cli
mobiscroll login --global
#F6GbSGa4eP8NTjq
mobiscroll config javascript

#ONE TIME RUBY CONFIGURATION
cd src/scripts/node/ps/
sudo apt install ruby-full ruby-dev g++ make build-essential zlibc zlib1g zlib1g-dev
git clone https://github.com/sagmor/sii_chile.git
sudo gem install sii_chile
ruby test.rb

#CREATE PFX
openssl pkcs12 -export -out certificate.pfx -inkey private.key -in certificate.crt -certfile ca_bundle.crt

#REQUIREMENTS WEBPAY NODEJS
sudo apt install python2

#PHP
sudo apt install curl php-cli php-mbstring git unzip
php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');"
php -r "if (hash_file('sha384', 'composer-setup.php') === 'a5c698ffe4b8e849a443b120cd5ba38043260d5c4023dbf93e1558871f1f07f58274fc6f4c93bcfd858c6bd0775cd8d1') { echo 'Installer verified'; } else { echo 'Installer corrupt'; unlink('composer-setup.php'); } echo PHP_EOL;"
php composer-setup.php --install-dir=/usr/local/bin --filename=composer
php -r "unlink('composer-setup.php');"
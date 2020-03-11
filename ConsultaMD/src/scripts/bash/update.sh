#one time ssh key config
#on notebook
ssh-keygen
type C:\Users\Guillermo\.ssh\id_rsa.pub | ssh root@45.7.230.240 -p 22222 "umask 077; test -d .ssh || mkdir .ssh ; cat >> .ssh/authorized_keys || exit 1"
#h5L2XvipyM37MYrK85

#change to EN-NZ
sudo apt install language-pack-en language-pack-gnome-en
sudo locale-gen en_NZ
sudo locale-gen en_NZ.UTF-8
sudo apt install $(check-language-support)
sudo dpkg-reconfigure locales
sudo update-locale LANG="en_NZ.UTF-8" LANGUAGE="en_NZ.UTF-8" LC_MESSAGES="en_NZ.UTF-8" LC_COLLATE="en_NZ.UTF-8" LC_CTYPE="en_NZ.UTF-8"
echo 'unset GREETER_LANGUAGE' >> ~/.profile

#ONE TIME DOTNET
#wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
#sudo dpkg -i packages-microsoft-prod.deb
#rm packages-microsoft-prod.deb
sudo add-apt-repository universe
#sudo apt update
#sudo apt upgrade
sudo apt install -y apt-transport-https g++ make build-essential zlibc zlib1g zlib1g-dev
#sudo apt purge aspnetcore-runtime-2.2 dotnet-runtime-2.2 dotnet-hostfxr-2.2 dotnet-host dotnet-runtime-deps-2.2 dotnet-sdk-2.2
#sudo apt install aspnetcore-runtime-2.2=2.2.5-1 dotnet-runtime-2.2=2.2.5-1 dotnet-hostfxr-2.2=2.2.5-1 dotnet-host=2.2.5-1 dotnet-runtime-deps-2.2=2.2.5-1 dotnet-sdk-2.2=2.2.300-1
#sudo apt-mark hold aspnetcore-runtime-2.2=2.2.5-1 dotnet-runtime-2.2=2.2.5-1 dotnet-hostfxr-2.2=2.2.5-1 dotnet-host=2.2.5-1 dotnet-runtime-deps-2.2=2.2.5-1 dotnet-sdk-2.2=2.2.300-1

#MULTIPLE ALONG SIDE
wget https://dot.net/v1/dotnet-install.sh
chmod 777 dotnet-install.sh
#install latest dotnet
./dotnet-install.sh -c 3.1 -Arch x64
#install dotnet core 2
./dotnet-install.sh -c 2.2 -Arch x64
echo 'export PATH="/root/.dotnet:$PATH"' >> ~/.bash_profile
echo 'export PATH="/root/.dotnet:$PATH"' >> ~/.bashrc
source ~/.bashrc
mv dotnet-install.sh /root/.dotnet

#ONE TIME MSSQL
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/msodbcsql17/
#https://packages.microsoft.com/ubuntu/16.04/mssql-server-2017/pool/main/m/mssql-server/
#https://packages.microsoft.com/ubuntu/16.04/prod/pool/main/m/mssql-tools/
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list)"
sudo apt update
#sudo apt purge msodbcsql17 mssql-server mssql-tools
sudo apt install msodbcsql17=17.3.1.1-1 mssql-server=14.0.3192.2-2 mssql-tools=17.3.0.1-1
sudo apt-mark hold msodbcsql17=17.3.1.1-1 mssql-server=14.0.3192.2-2 mssql-tools=17.3.0.1-1
sudo /opt/mssql/bin/mssql-conf setup
systemctl status mssql-server --no-pager

#ONE TIME MSSQL TOOLS
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list
sudo apt update
sudo apt install mssql-tools unixodbc-dev
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
sqlcmd -S localhost -U SA -P 34erdfERDF
systemctl restart mssql-server

#ONE TIME INSTALL LIBMAN
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
echo 'export PATH="/root/.dotnet/tools:$PATH"' >> ~/.bash_profile
echo 'export PATH="/root/.dotnet/tools:$PATH"' >> ~/.bashrc
echo 'export DOTNET_ROOT="/home/user01/.dotnet"' >> ~/.bash_profile
echo 'export DOTNET_ROOT="/home/user01/.dotnet"' >> ~/.bashrc
source ~/.bashrc
#ONE TIME INSTAL EF
#for core 2
#dotnet add package Microsoft.EntityFrameworkCore.Design
#dotnet --list-sdks
#dotnet new globaljson --sdk-version 2.2.402
#for core 1
#dotnet add package Microsoft.EntityFrameworkCore.Design -v 1.1.6
dotnet tool install --global dotnet-ef --version 3.1.1

#ONE TIME INSTALL NODEJS
sudo apt-get install gcc g++ make
curl -sL https://deb.nodesource.com/setup_12.x | sudo -E bash -
sudo apt install -y nodejs

#ONE TIME CHROMIUM
sudo apt install chromium-browser
sudo apt install gconf-service libasound2 libatk1.0-0 libatk-bridge2.0-0 libc6 libcairo2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils wget

#ONE TIME INSTALL NGINX
sudo apt install nginx
sudo systemctl start nginx

#ONE TIME INSTALL CERTBOT
sudo apt install software-properties-common
sudo add-apt-repository ppa:certbot/certbot
sudo apt update

sudo apt install certbot python-certbot-nginx

sudo certbot --nginx

#NGINX
sudo tee << EOL /etc/nginx/sites-available/consultamd >/dev/null
server {
    listen 80;
    server_name apdoc.cl www.apdoc.cl;
    location / {
        proxy_pass              http://localhost:5000;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection keep-alive;
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
        proxy_set_header        X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header        X-Forwarded-Proto \$scheme;
    }
    location /feedbackHub {
        proxy_pass              http://localhost:5000;
        proxy_http_version      1.1;
        proxy_set_header        Upgrade \$http_upgrade;
        proxy_set_header        Connection "upgrade";
        proxy_set_header        Host \$host;
        proxy_cache_bypass      \$http_upgrade;
    }
}
EOL

sudo ln -s /etc/nginx/sites-available/consultamd /etc/nginx/sites-enabled/consultamd

sudo /usr/sbin/nginx -s reload

#set up sftp
#https://guides.wp-bullet.com/adding-sftp-user-correct-permissions-nginx-php-fpm/
#https://www.emiprotechnologies.com/technical_notes/odoo-technical-notes-59/post/install-and-configure-vsftpd-475
sudo useradd -d /var/www/ ftpuser
sudo passwd ftpuser
sudo usermod -g www-data ftpuser
sudo nano /etc/nginx/nginx.conf
#replace user by user name
sudo nginx -t
sudo chown -R ftpuser:www-data /var/www
sudo find /var/www/ -type d -exec chmod 775 {} +
sudo find /var/www/ -type f -exec chmod 664 {} +
sudo service nginx restart
sudo apt install vsftpd
sudo vim /etc/vsftpd.conf
#write_enable=YES
#chroot_local_user=YES
#chroot_list_enable=NO
#chroot_list_file=/etc/vsftpd.chroot_list
sudo vim /etc/vsftpd.userlist
#allow_writeable_chroot=YES
sudo vim /etc/ssh/sshd_config
Subsystem sftp internal-sftp
#Match USER <user_name>
#        ChrootDirectory <user's home directory>
#        ForceCommand internal-sftp
#        X11Forwarding no
#        AllowTcpForwarding no
sudo systemctl restart vsftpd

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
#yarn install
./node_modules/cldr-data-downloader/bin/download.sh -f -i http://www.unicode.org/Public/cldr/26/json.zip -o ./wwwroot/lib/cldr-data
dotnet run
dotnet publish -r linux-x64 -c Release
systemctl stop kestrel-consultamd.service
rm -r /root/webapps/consultamd
mkdir -p /root/webapps/consultamd
rsync -auv bin/Release/netcoreapp3.1/linux-x64/publish/* /root/webapps/consultamd
npm --prefix /root/webapps/consultamd/ install /root/webapps/consultamd/
mkdir /root/webapps/consultamd/src/scripts/node/ps/trainset
mkdir /root/webapps/consultamd/src/scripts/node/mi/trainset
systemctl start kestrel-consultamd.service

#config kestrel
sudo tee << EOL /etc/systemd/system/kestrel-consultamd.service >/dev/null
[Unit]
Description=ConsultaMD

[Service]
WorkingDirectory=/root/webapps/consultamd
ExecStart=/usr/bin/dotnet ConsultaMD.dll
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-consultamd
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL

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

#TESTING NODE
delete require.cache[require.resolve('./FonasaService.js')];
var T = require('./FonasaService.js');
T(function (e, r) { console.log(e, r); }, { acKey: '693c4e031bcd23937811cedd2f1dba08', rut: '16124902-5' });

var T = require('./RegCivil.js');
T(function (e, r) { console.log(e, r); }, { acKey: '693c4e031bcd23937811cedd2f1dba08', rut: '16124902-5', carnet: '519194461' });

#SEED DB MANUALLY
cp /root/ConsultaMD/ConsultaMD/Data/*/*.tsv /tmp
sqlcmd -S localhost -U SA -P 34erdfERDF -d aspnet-ConsultaMD-35791A7D-6EDA-458B-88E6-9D9091ED2D7E -i seed.sql

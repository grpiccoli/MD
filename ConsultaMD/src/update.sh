cd /media/guillermo/WD3DNAND-SSD-1TB/
git clone https://github.com/grpltda/ConsultaMD.git
cd /media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/
git pull
cd ConsultaMD/
libman restore
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
./node_modules/cldr-data-downloader/bin/download.sh -i http://www.unicode.org/Public/cldr/26/json.zip -o ./wwwroot/lib/cldr-data
dotnet publish -r linux-x64 -c Release
systemctl stop kestrel-consultamd.service;rm -r ../../webapps/consultamd/;mkdir -p ../../webapps/consultamd/;rsync -auv bin/Release/netcoreapp2.2/linux-x64/publish/* ../../webapps/consultamd/;systemctl start kestrel-consultamd.service


sudo apt install ruby-full ruby-dev g++ make build-essential zlibc zlib1g zlib1g-dev
git clone https://github.com/sagmor/sii_chile.git
sudo gem install sii_chile
ruby test.rb
cd /media/guillermo/WD3DNAND-SSD-1TB/
git clone https://github.com/grpltda/ConsultaMD.git
cd /media/guillermo/WD3DNAND-SSD-1TB/ConsultaMD/
git pull
cd ConsultaMD/
dotnet bundle
libman restore
npm i
./node_modules/cldr-data-downloader/bin/download.sh -i http://www.unicode.org/Public/cldr/26/json.zip -o ./wwwroot/lib/cldr-data
dotnet publish -r linux-x64 -c Release
systemctl stop kestrel-consultamd.service;rm -r ../../webapps/consultamd/;mkdir -p ../../webapps/consultamd/;rsync -auv bin/Release/netcoreapp2.2/linux-x64/publish/* ../../webapps/consultamd/;systemctl start kestrel-consultamd.service
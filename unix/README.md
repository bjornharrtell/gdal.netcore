# Building GDAL bindings on unix

## Prerequisites

First of all I wish you to be patient & bring your snacks. Compilation from scratch takes nearly for 2 hours.

**Environment**: I'm compiling in WSL on **CentOS 7 with GLIBC 2.17 (2012)**

1. Dynamic linking
``` shell
yum install patchelf -y
```
2. Install dev tools > [link](https://github.com/microsoft/vcpkg#installing-linux-developer-tools)
``` shell
yum install centos-release-scl -y
yum install devtoolset-7 -y
scl enable devtoolset-7 bash
```
3. Install tools (required swig 3.0.12)
```shell
yum install swig3 -y
yum install automake -y
yum install libtool -y
yum install make -y
```

4. Install libraries
``` shell 
yum install xz-devel hdf-devel hdf5-devel libtiff sqlite-devel expat curl-devel -y
```

## Easy way compiling 

```shell
# don't forget to enable ONCE dev tools on CentOS 
scl enable devtoolset-7 bash
# install libraries with VCPKG
make -f vcpkg-makefile
# install main libraries (proj,geos,gdal)
make -f gdal-makefile
# generate bindings 
make 
# create packages
make pack
# test packages
make test
```

## Debian 11 w/ FileGDB

```sh
sudo su - root
apt update
apt install build-essential git autoconf libtool patchelf sqlite3 expat libcurl4-openssl-dev curl zip unzip tar libproj-dev
wget https://github.com/Esri/file-geodatabase-api/raw/master/FileGDB_API_1.5.2/FileGDB_API-RHEL7-64gcc83.tar.gz
tar xvfz FileGDB_API-RHEL7-64gcc83.tar.gz
# need to remove conflicting stdc libs
rm /root/FileGDB_API-RHEL7-64gcc83/lib/libstdc*
echo '/root/FileGDB_API-RHEL7-64gcc83/lib' >/etc/ld.so.conf.d/fgdb.conf
ldconfig
git clone https://github.com/MaxRev-Dev/gdal.netcore.git
cd gdal.netcore/unix
# remove hdf and curl support
# change to shared/GdalCore.opt VCPKG_REQUIRE_UNIX_DYNAMIC=libpq
# remove references to hdf curl in gdal-makefile
make -f vcpkg-makefile
# add --with-fgdb=/root/FileGDB_API-RHEL7-64gcc83 \ to gdal-makefile at appropriate place
# go to source-gdal and git checkout v3.3.3
make -j4 -f gdal-makefile
# fix RID.opt change to DEL_DIR=rm -rf
make -j4 -f gdal-makefile gdal
# edit vcproj files if custom version wanted
make
make pack
```
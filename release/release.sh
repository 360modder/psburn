#!/bin/bash
# envoirnment: ubuntu-18.04


# Global Variables
tools_install_path="../psburn"
psburn_version="1.0.0"
release_dir="psburn.$psburn_version.releases"
build_counts=$((build_counts+0))

echo_green () { # text
    echo -e "\033[0;32m$1\e[0m"
}

echo_red () { # text
    echo -e "\033[0;31m$1\e[0m"
}

install_tool () { # toolname
    if [ -e $1 ]
    then
        echo_green "[$(date '+%T %F')] $1 is already installed"
    else
        dotnet tool install $1 --tool-path $tools_install_path
        ./$1 install
    fi
}

create_package () { # package, runtime_identifier
    file_extension=$1
    created_package_path="../psburn/bin/Release/netcoreapp3.1/$2/psburn.$psburn_version.$2.$file_extension"
    error_message="Creating $file_extension package for $2 failed. View last executed command for errors."

    echo ""
    echo "[$(date '+%T %F')] Creating $file_extension package for $2"
    echo "[$(date '+%T %F')] Editing psburn.csproj"

    if python3 edit_csproj.py $2; then
        echo_green "[$(date '+%T %F')] psburn.csproj edited successfully"
    else
        echo_red "[$(date '+%T %F')] $error_message"
        exit 1
    fi
    

    if [ "$file_extension" == "tar.gz" ]; then
        echo "[$(date '+%T %F')] Running dotnet-tarball"
        cd $tools_install_path && ./dotnet-tarball
    else
        echo "[$(date '+%T %F')] Running dotnet-$file_extension"
        cd $tools_install_path && ./dotnet-$file_extension
    fi

    cd ../release

    if [ -e $created_package_path ]
    then
        cp $created_package_path "$release_dir/psburn.$psburn_version.$2.$file_extension"
        echo_green "[$(date '+%T %F')] psburn.$psburn_version.$2.$file_extension package created and copied to $release_dir"
    else
        echo_red "[$(date '+%T %F')] $error_message"
        exit 1
    fi

    ((build_counts++))
}

create_exe () {
    echo ""

    if [ -e "Inno Setup 6" ]
    then
        echo_green "[$(date '+%T %F')] Inno Setup 6 is already installed"
    else
        echo "[$(date '+%T %F')] Installing Inno Setup 6"
        wget --load-cookies /tmp/cookies.txt "https://docs.google.com/uc?export=download&confirm=$(wget --quiet --save-cookies /tmp/cookies.txt --keep-session-cookies --no-check-certificate 'https://docs.google.com/uc?export=download&id=103QGKdC60IBXhKH2nAW0PerulLHZaINa' -O- | sed -rn 's/.*confirm=([0-9A-Za-z_]+).*/\1\n/p')&id=103QGKdC60IBXhKH2nAW0PerulLHZaINa" -O "Inno Setup 6.zip" && rm -rf /tmp/cookies.txt
        unzip "Inno Setup 6.zip"
        rm "Inno Setup 6.zip"
    fi
    
    if ! [ -x "$(command -v wine)" ]; then
        echo "[$(date '+%T %F')] Installing Wine"
        sudo dpkg --add-architecture i386
        sudo apt update
        sudo apt install wine64 wine32
    fi
        echo_green "[$(date '+%T %F')] Wine is already installed"

    echo "[$(date '+%T %F')] Creating setup package for win-x64"
    echo "[$(date '+%T %F')] Editing psburn.csproj"

    if python3 edit_csproj.py win-x64; then
        echo_green "[$(date '+%T %F')] psburn.csproj edited successfully"
    else
        echo_red "[$(date '+%T %F')] $error_message"
        exit 1
    fi

    echo "[$(date '+%T %F')] Running dotnet publish"
    cd ../psburn && dotnet publish
    cd ../release

    echo "[$(date '+%T %F')] Running iscc.exe"
    wine "Inno Setup 6/iscc.exe" setup.iss

    if [ -e "psburn.$psburn_version.win-x64.exe" ]
    then
        cp "psburn.$psburn_version.win-x64.exe" "$release_dir/psburn.$psburn_version.win-x64.exe"
        echo_green "[$(date '+%T %F')] psburn.$psburn_version.win-x64.exe setup package created and copied to $release_dir"
    else
        echo_red "[$(date '+%T %F')] Creating setup package for win-x64 failed. View last executed command for errors."
        exit 1
    fi

    ((build_counts++))
}


if ! [ -x "$(command -v dotnet)" ]; then
    echo "[$(date '+%T %F')] Installing .NET Core SDK & Runtime 3.1"
    wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update; \
        sudo apt-get install -y apt-transport-https && \
        sudo apt-get update && \
        sudo apt-get install -y dotnet-sdk-3.1
    sudo apt-get install -y dotnet-runtime-3.1
    rm -rf packages-microsoft-prod.deb
fi
    echo_green "[$(date '+%T %F')] .NET Core is already installed"

echo "[$(date '+%T %F')] Installing and adding .NET packaging tools to project"
cd $tools_install_path
install_tool dotnet-zip
install_tool dotnet-tarball
install_tool dotnet-rpm
install_tool dotnet-deb
cd ../release

echo "[$(date '+%T %F')] Creating releases at $release_dir"
[ -d $release_dir ] && rm -rf $release_dir # If release directory exists delete it and create a new directory
mkdir $release_dir

# Zip Releases
create_package zip win-x64
create_package zip win-x86
# Tarball Releases
create_package tar.gz linux-x64
create_package tar.gz osx-x64
# Rpm Releases
create_package rpm linux-x64
# Deb Releases
create_package deb linux-x64
# Setup Releases
create_exe

echo ""
echo_green "[$(date '+%T %F')] Total $build_counts packages were created"

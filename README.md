# Running Program on a Raspberry Pi
By running this application on a Raspberry Pi on an Open Port, it allows any device connected to the same network to access the Music Streamer (including Mobile).
- The following steps have been made to run on a Raspberry Pi 4B, however could be used on any UNIX based system.

**DotNet:**
```
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
`chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest` (8.0.4 as of this time)
./dotnet-install.sh --version latest --runtime aspnetcore
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
```

**MariaDB Installation:** 
```
sudo apt install mariadb-server mariadb-client
sudo systemctl start mariadb
sudo systemctl start mariadb
sudo systemctl enable mariadb
sudo mysql_secure_installation
```

**MariaDB Configuration:**
```
sudo mysql
CREATE DATABASE LocalMusicStreamer;
CREATE USER 'NormalUser'@'localhost' IDENTIFIED BY 'LocalPassword';
GRANT ALL PRIVILEGES ON LocalMusicStreamer.* TO 'NormalUser'@'localhost';
FLUSH PRIVILEGES;
```

**Project Install:**
```
git clone https://github.com/BradleyBeamish/LocalMusicStreamer.git
cd LocalMusicStreamer
mkdir -p ~/LocalMusicStreamer/wwwroot/uploads
```

**Project Dependencies:**
```
dotnet add package System.Diagnostics.Debug --version 4.3.0
dotnet add package System.IO.FileSystem --version 4.3.0
dotnet add package System.IO.FileSystem.Primitives --version 4.3.0
dotnet add package System.Net.Primitives --version 4.3.0
```

**Fill Database from Project:**
```
dotnet tool install --global dotnet-ef
dotnet ef migrations add NewMigration
dotnet ef database update
```

**Build Project:**
```
dotnet restore
dotnet build
dotnet publish -c Release -r linux-arm64 --self-contained=false -o ./publish
```

*Run Locally:* `dotnet /home/terra5123/LocalMusicStreamer/publish/LocalMusicStreamer.dll`

*Run on Port:* `dotnet /home/terra5123/LocalMusicStreamer/publish/LocalMusicStreamer.dll --urls "http://0.0.0.0:5000"`

Access site at: http://[hostIP]:5000/ 

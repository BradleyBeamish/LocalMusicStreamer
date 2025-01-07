# Local Music Streamer
In today's day of age, almost everyone is subscribed to multiple subscription based services. They offer a simplified way to access media, for a monthly cost. However, the user will be stuck with lower quality content that they will not own.

This Music Streaming Application was developed to switch away from the big music streaming services. It provides a user interface to listen to your high quality audio files, without any extra costs.

**Benefits:**
- High Quality Music
- As the user owns their audio files, they cannot be taken away.
- Everything is done locally, ensuring the best performance and security.
***
# Compression
One benefit to having your own music files, is the ability to have lossless quality. This is not always available in streaming services.
- The one downside to having the highest quality audio files, is their filesize.
- This is why the application allows for Audio Compression.
- Using FFmpeg, the user can choose to compress a file to a specific bitrate.
- In the photo below, a 27MB audio file was compressed down to 5MB, both files will sound the same to casual listeners.

![Screenshot From 2025-01-07 11-17-33](https://github.com/user-attachments/assets/87446f51-9e1c-4abf-a01d-c0d78720d3e9)
***
# User Interface Overview
![Screenshot From 2025-01-07 11-16-56](https://github.com/user-attachments/assets/f57db6be-928a-433f-be6d-b27030fb45db)
![Screenshot From 2025-01-07 11-14-24](https://github.com/user-attachments/assets/6e33a0c7-d4bd-46d7-8bc1-3d56082f07f0)
![Screenshot From 2025-01-07 11-15-18](https://github.com/user-attachments/assets/f07c8f8f-32cf-4450-afd5-5e8fdea19eb6)
![Screenshot From 2025-01-07 11-15-56](https://github.com/user-attachments/assets/9b8b1b1d-35fb-4741-9a28-43c9b11bbb43)
![Screenshot From 2025-01-07 11-16-24](https://github.com/user-attachments/assets/a04c0f05-a5f9-40aa-9a11-7fd9b36d29b4)
![Screenshot From 2025-01-07 11-16-40](https://github.com/user-attachments/assets/e462f86c-4d19-4b89-a85c-f5d04ac21e89)
***
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

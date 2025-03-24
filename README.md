# Auto Update Tool

A Windows console application written in C# for automatic software updates.

## Features

- Check local version from `version.txt`
- Check remote version from configurable URL
- Compare versions using semantic versioning
- Download and install updates from `update.zip`
- Skip files/directories listed in `ignore.txt`

## Usage

1. Create `version.txt` in the same directory as the program, containing the current version (e.g., `1.0.1`)

2. Create `ignore.txt` (optional) to list files or directories to skip during updates:
```
config.json
userdata/
```

3. Configure URLs in `Program.cs`:
   - `remoteVersionUrl`: URL to check for new version (e.g., `https://example.com/version.txt`)
   - `updateUrl`: URL to download update.zip (e.g., `https://example.com/update.zip`)

   Example:
   ```csharp
   string remoteVersionUrl = "https://example.com/version.txt";
   string updateUrl = "https://example.com/update.zip";
   ```

4. Run the program or start build:
```
dotnet run
```

## Directory Structure

```
YourApp/
├── Update.exe
├── version.txt
├── ignore.txt
└── [other application files]
```

## Notes

- `version.txt` must contain a valid version number (e.g., `1.0.1`)
- `update.zip` must contain files to update with the same directory structure as the target
- Files listed in `ignore.txt` will be preserved during updates
- Make sure the URLs are accessible and the remote `version.txt` contains a valid version number 
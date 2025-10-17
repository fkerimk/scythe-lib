# SCYTHE <img width="24" height="24" alt="icon" src="https://fkerimk.com/scythe/icon.png" /> lib

Common Scythe library for the util, core, and run modules.

## 🛠 License

scythe-lib is licensed under the [LGPL-2.1 license](./LICENSE).

## Building

Make sure you have .NET SDK 3.0 or higher installed

```bash
git clone https://github.com/fkerimk/scythe-lib.git
cd ./scythe-lib/
dotnet publish -v q -c Release
cd ..
```

The build will be at the `./scythe-lib/bin/Release/netstandard2.1/publish/scythe-lib.dll`.


You can take an automatic build using the [scythe-build](https://github.com/fkerimk/scythe-build) module.

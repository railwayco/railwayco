# RailwayCo
![RailwayCo](/Images/logo_1024x128.png "RailwayCo")

## About
A Unity 2D game about becoming a railway tycoon by managing trains and cargo in a bid to link up the world

### Tech Stack
- Unity
- Aseprite
- Microsoft Azure PlayFab
- `<!-- Insert Deployment Tool -->`

## Getting Started
This project contains PlayFab Unity SDK as a submodule and will require additional steps to install the SDK Unity packages.

### Installation
1. Clone the repository and its submodules.
```sh
$ git clone --recurse-submodules https://github.com/railwayco/railwayco.git
```
2. If you already cloned the project and forgot `--recurse-submodules`, you can `Clone` the submodule into the repository with the following commands:
```sh
$ git submodule update --init
```
3. Install [Unity Hub](https://unity.com/download).
4. Install the `PlayFab SDK` package.
```sh
$ /path/to/Unity -projectPath RailwayCo/ -importPackage UnitySDK/Packages/UnitySDK.unitypackage
```
5. (Optional) Install the `PlayFab Editor Extensions` package.
```sh
$ /path/to/Unity -projectPath RailwayCo/ -importPackage UnitySDK/Packages/PlayFabEditorExtensions.unitypackage
```

## Usage
`<!-- Insert usage -->`

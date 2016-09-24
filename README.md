# <img width="48px" src="https://raw.githubusercontent.com/Muhammad-Mohsen/Minimalist-Music-Player/master/Source/MinimalistMusicPlayer/Resources/img/AppIcon.ico"> Minimalist Music Player
Simple WPF music player.

Minimalist Music Player is a light weight, clean music player made in WPF (although there's no MVVM involved). It's by no means a fleshed-out player; There's no playlist management, no equalizer, etc. 

## Functionality
+ Plays **mp3**, **wma**, and **wav** files.
+ Intergrated explorer complete with a simple breadcrumb bar.
+ On-the-fly playlists: select some/all music files in a given directory -- play selected files
+ Shuffle.
+ Repteat.
+ Stay on top.

## Some technical details
- No MVVM was harmed in the making of this app.
- All the icons are vectors, and are read from a resx file -- Original SVGs are included in the repo.
- playback/playlist functionality is provided by the WMPLib.dll
- I tried using partial classes to try and keep everything organized.

## Screenshots
Collapsed

<img src="https://raw.githubusercontent.com/Muhammad-Mohsen/Minimalist-Music-Player/master/Doc/Screenshots/MinimalistCollapsed.png">

Expanded

<img src="https://raw.githubusercontent.com/Muhammad-Mohsen/Minimalist-Music-Player/master/Doc/Screenshots/MinimalistExpanded.png">

On-the-fly playlist 

<img src="https://raw.githubusercontent.com/Muhammad-Mohsen/Minimalist-Music-Player/master/Doc/Screenshots/MinimalistSelectTracks.png">

## License and copyright

Copyright (c) 2016 Muhammad Mohsen

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

<http://opensource.org/licenses/MIT/>
# SiHan.Libs.Image

## 介绍

.net 图片处理类，基于netstandard2.0，通过一个类`ImageWrapper`，提供以下功能：

- 创建验证码图片
- 图片保存为文件
- 调整图片尺寸大小
- 裁剪图片
- 判断一个文件是否是真实图片

## 安装

```
PM> Install-Package SiHan.Libs.Image
```



## Linux部署

linux环境下必须上传[libSkiaSharp.so](https://github.com/mono/SkiaSharp/releases/tag/v1.68.0)文件在项目目录内。


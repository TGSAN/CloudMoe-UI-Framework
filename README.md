# CloudMoe UI Framework 云萌UI框架

Simply to building your WPF look like UWP apps. (even on Windows 7 or Vista)

![UI界面截图][UI_image]

![亚克力与普通模糊对比][AcrylicDemo_image]

# Features 功能

### General 一般

- [x] 高兼容性 最低兼容 Windows Vista 最佳使用体验 Windows 10 1803 及以上 (Windows Vista 需要将框架版本降为4.6)

- [x] 亚克力模糊透明

- [x] 亚克力模糊焦点移除时的过渡动效

- [x] 效果自动降级（降级优先级为：亚克力模糊 > 旧 Windows 10 标准模糊 > Aero 模糊 > 半透明）

- [x] Windows 10 Style 在任意系统上（Vista~10）

- [x] 设计器直接编辑标题栏

- [x] 仿Qt改变窗体动画

- [x] MahApps兼容

- [x] 支持主动跟随系统默认应用主题色（Windows 10 Light Theme or Dark Theme）

- [x] 主动避让 Windows Vista or Windows 7 禁用 DWM 的 Failback

- [x] Fluent Design System 材质 (Material)

- [ ] Fluent Design System 光感 (Light)

- [ ] Fluent Design System 动效 (Motion)

- [ ] Fluent Design System 深度 (Depth)

### Blur Effect Level 模糊特性等级

| 系统                      | 特效                           |
| :------------------------ |:------------------------------|
| Windows 10 1803~1809+     | 亚克力模糊 (Acrylic Blur)      |
| Windows 10 1507~1709      | Windows 10 模糊 (Acrylic Blur) |
| Windows 8~8.1             | 半透明 (Translucent)           |
| Windows Vista~7           | Aero模糊 (Acrylic Blur)        |

# License 许可协议

[Apache License 2.0](./LICENSE)

# Thanks 感谢

MahApps/MahApps.Metro

ControlzEx/ControlzEx

bbougot/AcrylicWPF

murrple-1/APNGManagement

riverar/sample-win32-acrylicblur

[UI_image]:./Pages/images/Screen.png
[AcrylicDemo_image]:./Pages/images/AcrylicDemo.png

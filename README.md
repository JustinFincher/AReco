# AReco

Rank 2 @ Hunan huazhong HHHackathon (Hosted by HNU)
湖南 华中 HHHackathon 二等奖 （湖南大学举办）

# Build

因为使用了 Asset Store 上的第三方素材，某些素材的 license 不允许我们完全开源，因此在开源的时候删除了对应内容。  
项目在目前已有代码外所需 Package 如下：

- Beautify (非必须)
- ConsolePro (非必须)
- HdgRemoteDebug (非必须)
- Advanced FPS Counter (非必须)
- SRDebugger (非必须)
- ScriptInspector3 (非必须)
- TaskParallel（忘了有没有用到了，如果用到了就是必须）
- Vectrosity（必须）

ImageRecoManager 内 APPKEY 也请自行填写，导出 Xcode 项目后请将对应的 mlmodel 拖至 Xcode Compile 阶段的列表里后再 Build
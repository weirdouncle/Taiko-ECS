# Taiko-ECS
一个基于ECS-DOTS控制音符的实验性太鼓模拟器，目前游戏主逻辑未完成，仅完成主要音符（1～7）生成
UNITY版本2022.3.60f1，音频使用FMOD，NOTE音使用Criware
## 如何调试
1. 点击播放，等待一会，当谱面读取完成后会出现按钮；
2. 点击“DOTS”按钮开始生成基于DOTS系统和MeshRender的音符Entities演示；
3. 点击“Legacy”按钮开始基于传统SpriteRenderer的GameObject演示;
4. 你可以打开Hierarchy下的OtherObject/Load，替换其中LoaderScript脚本的SongTja为你的其他谱面，以及OtherObject/SoundPool替换MusicLoaderScript的Music为你的歌曲；
## ECS-DOTS对比GameObject的优劣势
1. 当音符数量足够多时，ECS-DOTS不仅在渲染以及CPU效率上都有明显优势，逻辑处理能够使用多线程和burst编译，Mesh渲染合批减少drawcall
2. ECS是面向数据编程，不如GameObject面向对象直观，难度很大
3. Entity的渲染基于entities.graphics，尽管支持SpriteRenderer，但是属于负优化，drawcall会爆炸
4. 在2D游戏中使用MeshRender渲染2D Sprite有很多痛点，如果希望像SpriteRenderer一样完美的渲染出半透明像素，则必须ZWrite Off，但MeshRender不会基于Z值调整渲染顺序，Entity之间的遮挡存在严重问题，如果开启ZWrite并进行透明度测试，半透明像素部分亦会完全遮挡

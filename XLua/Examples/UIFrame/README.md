﻿

## 关于Lua的面向过程编程模板

Lua只定义当前UI窗口或控件的事件和功能函数，而UI窗口功能的数据由MonoBehaviour继承来的FuncLuaBehavior的Awake中定义，在OnDestroy时销毁。数据作为参数递到Lua事件，在Lua事件响应中处理对应的UI组件。

### 为什么不面向对象，而要面向过程？

* 面向过程比面向对象简单清晰，容易上手，而UI功能也比较简单，就是事件响应，用面向过程就可以解决问题
* Lua的面向对象基于metatable实现，写一个类声明就需要好多行代码，相比高级语言的class声明还复杂
* Lua中定义的变量的作用域有全局和当前文件，但只能对应一个C#对象，如果C#的多个对象，就需要定义多个变量与之对应，实现背包这样的功能比较麻烦。使用面向过程，数据和功能分离，Lua中实现功能，数据由C#定义和管理。数据和UI组件的生命周期对应，Awake时定义，OnDestroy时销毁。数据作为参数递到Lua事件，在Lua事件响应中处理对应的UI组件。




tools = {}
tools.PI = 3.14

local function fun() -- 普通函数 local=私有
    print('fun')
end

function tools.fun() -- 成员变量 无local=共有
    print('tools.fun()')
end

function tools:fun1() -- 成员函数
    print('tools:fun()1')
end

function tools:callFun()
    fun();
end

---------------------------- metatable
t = {}
meta = {}
t = setmetatable(t, meta);
getmetatable(t);
-----------------------------------------
t1 = {
    data = 10
}
t2 = {}
t3 = {}
m1 = {
    __index = {
        value = 20
    }
}
m2 = {
    __index = mt1,
    value = 30
}

setmetatable(t2, m1)
-- setmetatable(t3, mt2)
print(t2.data)
print(t2.value)
-----------------------------------------------
t = setmetatable({
    key1 = "value1"
}, {
    __index = function(this, key)
        if key == "key2" then
            return "metatablevalue"
        else
            return nil
        end
    end
})
print(t.key1, t.key2, t.key3) -- 这里的key2=="key2"
-----------------------------------------------------

local player = {
    name = "张三",
    id = 1,
    show = function()
        print(name .. id)
    end
}

local t = {}
local mt = {
    __index = player,
    ---- __newindex = function(t, key, value)
    ----    print(key .. key .. value)
    ---- end
    __newindex = player
}
setmetatable(t, mt)
t.name = "newValue"
print(t.name)
print(player.name)

return tools
-- __index 注意，原方法不代表是原表
-- __newindex

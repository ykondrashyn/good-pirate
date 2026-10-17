mergeInto(LibraryManager.library, {
  GoodPirate113StorageGetLength: function (keyPointer) {
    try {
      var key = "gp113:" + UTF8ToString(keyPointer)
      var value = window.localStorage.getItem(key)
      return value === null ? -1 : lengthBytesUTF8(value)
    } catch (error) {
      return -2
    }
  },

  GoodPirate113StorageRead: function (keyPointer, bufferPointer, bufferLength) {
    try {
      var key = "gp113:" + UTF8ToString(keyPointer)
      var value = window.localStorage.getItem(key)
      if (value === null || lengthBytesUTF8(value) + 1 > bufferLength) {
        return 0
      }

      stringToUTF8(value, bufferPointer, bufferLength)
      return 1
    } catch (error) {
      return 0
    }
  },

  GoodPirate113StorageWrite: function (keyPointer, payloadPointer) {
    try {
      var key = "gp113:" + UTF8ToString(keyPointer)
      window.localStorage.setItem(key, UTF8ToString(payloadPointer))
      return 1
    } catch (error) {
      return 0
    }
  },

  GoodPirate113StorageDelete: function (keyPointer) {
    try {
      var key = "gp113:" + UTF8ToString(keyPointer)
      window.localStorage.removeItem(key)
      return 1
    } catch (error) {
      return 0
    }
  }
})

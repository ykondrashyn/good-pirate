mergeInto(LibraryManager.library, {
  GoodPirate113ClipboardWrite: function (valuePointer, receiverPointer) {
    if (!window.isSecureContext || !navigator.clipboard ||
        !navigator.clipboard.writeText) return 0
    var value = UTF8ToString(valuePointer)
    var receiver = UTF8ToString(receiverPointer)
    navigator.clipboard.writeText(value).then(function () {
      SendMessage(receiver, "OnClipboardWrite", "ok")
    }).catch(function (error) {
      SendMessage(receiver, "OnClipboardWrite",
        error && error.name ? error.name : "denied")
    })
    return 1
  },

  GoodPirate113ClipboardRead: function (receiverPointer) {
    if (!window.isSecureContext || !navigator.clipboard ||
        !navigator.clipboard.readText) return 0
    var receiver = UTF8ToString(receiverPointer)
    navigator.clipboard.readText().then(function (value) {
      SendMessage(receiver, "OnClipboardRead", value)
    }).catch(function (error) {
      SendMessage(receiver, "OnClipboardReadFailed",
        error && error.name ? error.name : "denied")
    })
    return 1
  },

  GoodPirate113ClipboardManualCopy: function (valuePointer) {
    window.dispatchEvent(new CustomEvent("goodpirate113-manual-copy", {
      detail: { value: UTF8ToString(valuePointer) }
    }))
  },

  GoodPirate113ClipboardManualPaste: function () {
    window.dispatchEvent(new CustomEvent("goodpirate113-manual-paste"))
  },

  GoodPirate113ClipboardConfirmPaste: function (valuePointer) {
    window.dispatchEvent(new CustomEvent("goodpirate113-confirm-paste", {
      detail: { value: UTF8ToString(valuePointer) }
    }))
  },

  GoodPirate113ClipboardImportResult: function (succeeded, messagePointer) {
    window.dispatchEvent(new CustomEvent("goodpirate113-import-result", {
      detail: {
        succeeded: succeeded === 1,
        message: UTF8ToString(messagePointer)
      }
    }))
  }
})

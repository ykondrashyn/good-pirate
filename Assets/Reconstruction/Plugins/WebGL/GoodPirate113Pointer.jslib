mergeInto(LibraryManager.library, {
  GoodPirate113PointerInstall: function (receiverPointer) {
    if (window.__goodPirate113PointerInputInstalled) return;
    window.__goodPirate113PointerInputInstalled = true;

    var receiver = UTF8ToString(receiverPointer);
    var canvas = Module.canvas;
    var heldPointers = new Map();
    canvas.style.touchAction = "none";

    var clamp = function (value, minimum, maximum) {
      return Math.max(minimum, Math.min(maximum, value));
    };
    var fixed = function (value) {
      return Number.isFinite(value) ? value.toFixed(3) : "0";
    };
    var canvasPoint = function (clientX, clientY) {
      var rect = canvas.getBoundingClientRect();
      var width = Math.max(rect.width, 1);
      var height = Math.max(rect.height, 1);
      return {
        x: clamp((clientX - rect.left) * canvas.width / width, 0, canvas.width),
        y: clamp((rect.bottom - clientY) * canvas.height / height, 0, canvas.height)
      };
    };
    var payload = function (event, previous) {
      var point = canvasPoint(event.clientX, event.clientY);
      var oldPoint = canvasPoint(previous.clientX, previous.clientY);
      return [
        event.pointerId,
        fixed(point.x),
        fixed(point.y),
        fixed(point.x - oldPoint.x),
        fixed(point.y - oldPoint.y),
        typeof event.buttons === "number" ? event.buttons : 0,
        fixed(event.timeStamp)
      ].join(",");
    };
    var storedPayload = function (pointerId, state) {
      var point = canvasPoint(state.clientX, state.clientY);
      return [pointerId, fixed(point.x), fixed(point.y), "0", "0", "0", fixed(performance.now())].join(",");
    };
    var releaseCapture = function (pointerId) {
      try {
        if (canvas.hasPointerCapture(pointerId)) canvas.releasePointerCapture(pointerId);
      } catch (error) {
      }
    };

    canvas.addEventListener("pointerdown", function (event) {
      if (heldPointers.has(event.pointerId)) return;
      var state = { clientX: event.clientX, clientY: event.clientY };
      heldPointers.set(event.pointerId, state);
      try {
        canvas.setPointerCapture(event.pointerId);
      } catch (error) {
      }
      SendMessage(receiver, "OnBrowserPointerDown", payload(event, state));
    }, true);

    window.addEventListener("pointermove", function (event) {
      var state = heldPointers.get(event.pointerId);
      if (!state) return;
      var message = payload(event, state);
      state.clientX = event.clientX;
      state.clientY = event.clientY;
      SendMessage(receiver, "OnBrowserPointerMove", message);
    }, true);

    var finish = function (event, cancelled) {
      var state = heldPointers.get(event.pointerId);
      if (!state) return;
      SendMessage(receiver, cancelled ? "OnBrowserPointerCancel" : "OnBrowserPointerUp", payload(event, state));
      heldPointers.delete(event.pointerId);
      releaseCapture(event.pointerId);
    };
    window.addEventListener("pointerup", function (event) {
      finish(event, false);
    }, true);
    window.addEventListener("pointercancel", function (event) {
      finish(event, true);
    }, true);
    canvas.addEventListener("lostpointercapture", function (event) {
      var state = heldPointers.get(event.pointerId);
      if (!state) return;
      SendMessage(receiver, "OnBrowserPointerCancel", storedPayload(event.pointerId, state));
      heldPointers.delete(event.pointerId);
    }, true);

    var releaseAll = function (reason) {
      heldPointers.forEach(function (state, pointerId) {
        SendMessage(receiver, "OnBrowserPointerCancel", storedPayload(pointerId, state));
        releaseCapture(pointerId);
      });
      heldPointers.clear();
      SendMessage(receiver, "OnBrowserInputLostFocus", reason);
    };
    window.addEventListener("blur", function () {
      releaseAll("blur");
    });
    document.addEventListener("visibilitychange", function () {
      if (document.hidden) releaseAll("hidden");
    });
    window.addEventListener("pagehide", function () {
      releaseAll("pagehide");
    });
  }
});

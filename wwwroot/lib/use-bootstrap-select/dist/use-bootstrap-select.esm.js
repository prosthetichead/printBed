var __defProp = Object.defineProperty;
var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
var __publicField = (obj, key, value) => __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
const equalFn = (a, b) => a === b;
const $TRACK = Symbol("solid-track");
const signalOptions = {
  equals: equalFn
};
let runEffects = runQueue;
const STALE = 1;
const PENDING = 2;
const UNOWNED = {
  owned: null,
  cleanups: null,
  context: null,
  owner: null
};
var Owner = null;
let Transition = null;
let ExternalSourceConfig = null;
let Listener = null;
let Updates = null;
let Effects = null;
let ExecCount = 0;
function createRoot(fn, detachedOwner) {
  const listener = Listener, owner = Owner, unowned = fn.length === 0, current = detachedOwner === void 0 ? owner : detachedOwner, root = unowned ? UNOWNED : {
    owned: null,
    cleanups: null,
    context: current ? current.context : null,
    owner: current
  }, updateFn = unowned ? fn : () => fn(() => untrack(() => cleanNode(root)));
  Owner = root;
  Listener = null;
  try {
    return runUpdates(updateFn, true);
  } finally {
    Listener = listener;
    Owner = owner;
  }
}
function createSignal(value, options) {
  options = options ? Object.assign({}, signalOptions, options) : signalOptions;
  const s = {
    value,
    observers: null,
    observerSlots: null,
    comparator: options.equals || void 0
  };
  const setter = (value2) => {
    if (typeof value2 === "function") {
      value2 = value2(s.value);
    }
    return writeSignal(s, value2);
  };
  return [readSignal.bind(s), setter];
}
function createRenderEffect(fn, value, options) {
  const c = createComputation(fn, value, false, STALE);
  updateComputation(c);
}
function createEffect(fn, value, options) {
  runEffects = runUserEffects;
  const c = createComputation(fn, value, false, STALE);
  c.user = true;
  Effects ? Effects.push(c) : updateComputation(c);
}
function createMemo(fn, value, options) {
  options = options ? Object.assign({}, signalOptions, options) : signalOptions;
  const c = createComputation(fn, value, true, 0);
  c.observers = null;
  c.observerSlots = null;
  c.comparator = options.equals || void 0;
  updateComputation(c);
  return readSignal.bind(c);
}
function untrack(fn) {
  if (Listener === null) return fn();
  const listener = Listener;
  Listener = null;
  try {
    if (ExternalSourceConfig) ;
    return fn();
  } finally {
    Listener = listener;
  }
}
function onCleanup(fn) {
  if (Owner === null) ;
  else if (Owner.cleanups === null) Owner.cleanups = [fn];
  else Owner.cleanups.push(fn);
  return fn;
}
function readSignal() {
  if (this.sources && this.state) {
    if (this.state === STALE) updateComputation(this);
    else {
      const updates = Updates;
      Updates = null;
      runUpdates(() => lookUpstream(this), false);
      Updates = updates;
    }
  }
  if (Listener) {
    const sSlot = this.observers ? this.observers.length : 0;
    if (!Listener.sources) {
      Listener.sources = [this];
      Listener.sourceSlots = [sSlot];
    } else {
      Listener.sources.push(this);
      Listener.sourceSlots.push(sSlot);
    }
    if (!this.observers) {
      this.observers = [Listener];
      this.observerSlots = [Listener.sources.length - 1];
    } else {
      this.observers.push(Listener);
      this.observerSlots.push(Listener.sources.length - 1);
    }
  }
  return this.value;
}
function writeSignal(node, value, isComp) {
  let current = node.value;
  if (!node.comparator || !node.comparator(current, value)) {
    node.value = value;
    if (node.observers && node.observers.length) {
      runUpdates(() => {
        for (let i = 0; i < node.observers.length; i += 1) {
          const o = node.observers[i];
          const TransitionRunning = Transition && Transition.running;
          if (TransitionRunning && Transition.disposed.has(o)) ;
          if (TransitionRunning ? !o.tState : !o.state) {
            if (o.pure) Updates.push(o);
            else Effects.push(o);
            if (o.observers) markDownstream(o);
          }
          if (!TransitionRunning) o.state = STALE;
        }
        if (Updates.length > 1e6) {
          Updates = [];
          if (false) ;
          throw new Error();
        }
      }, false);
    }
  }
  return value;
}
function updateComputation(node) {
  if (!node.fn) return;
  cleanNode(node);
  const time = ExecCount;
  runComputation(
    node,
    node.value,
    time
  );
}
function runComputation(node, value, time) {
  let nextValue;
  const owner = Owner, listener = Listener;
  Listener = Owner = node;
  try {
    nextValue = node.fn(value);
  } catch (err) {
    if (node.pure) {
      {
        node.state = STALE;
        node.owned && node.owned.forEach(cleanNode);
        node.owned = null;
      }
    }
    node.updatedAt = time + 1;
    return handleError(err);
  } finally {
    Listener = listener;
    Owner = owner;
  }
  if (!node.updatedAt || node.updatedAt <= time) {
    if (node.updatedAt != null && "observers" in node) {
      writeSignal(node, nextValue);
    } else node.value = nextValue;
    node.updatedAt = time;
  }
}
function createComputation(fn, init, pure, state = STALE, options) {
  const c = {
    fn,
    state,
    updatedAt: null,
    owned: null,
    sources: null,
    sourceSlots: null,
    cleanups: null,
    value: init,
    owner: Owner,
    context: Owner ? Owner.context : null,
    pure
  };
  if (Owner === null) ;
  else if (Owner !== UNOWNED) {
    {
      if (!Owner.owned) Owner.owned = [c];
      else Owner.owned.push(c);
    }
  }
  return c;
}
function runTop(node) {
  if (node.state === 0) return;
  if (node.state === PENDING) return lookUpstream(node);
  if (node.suspense && untrack(node.suspense.inFallback)) return node.suspense.effects.push(node);
  const ancestors = [node];
  while ((node = node.owner) && (!node.updatedAt || node.updatedAt < ExecCount)) {
    if (node.state) ancestors.push(node);
  }
  for (let i = ancestors.length - 1; i >= 0; i--) {
    node = ancestors[i];
    if (node.state === STALE) {
      updateComputation(node);
    } else if (node.state === PENDING) {
      const updates = Updates;
      Updates = null;
      runUpdates(() => lookUpstream(node, ancestors[0]), false);
      Updates = updates;
    }
  }
}
function runUpdates(fn, init) {
  if (Updates) return fn();
  let wait = false;
  if (!init) Updates = [];
  if (Effects) wait = true;
  else Effects = [];
  ExecCount++;
  try {
    const res = fn();
    completeUpdates(wait);
    return res;
  } catch (err) {
    if (!wait) Effects = null;
    Updates = null;
    handleError(err);
  }
}
function completeUpdates(wait) {
  if (Updates) {
    runQueue(Updates);
    Updates = null;
  }
  if (wait) return;
  const e = Effects;
  Effects = null;
  if (e.length) runUpdates(() => runEffects(e), false);
}
function runQueue(queue) {
  for (let i = 0; i < queue.length; i++) runTop(queue[i]);
}
function runUserEffects(queue) {
  let i, userLength = 0;
  for (i = 0; i < queue.length; i++) {
    const e = queue[i];
    if (!e.user) runTop(e);
    else queue[userLength++] = e;
  }
  for (i = 0; i < userLength; i++) runTop(queue[i]);
}
function lookUpstream(node, ignore) {
  node.state = 0;
  for (let i = 0; i < node.sources.length; i += 1) {
    const source = node.sources[i];
    if (source.sources) {
      const state = source.state;
      if (state === STALE) {
        if (source !== ignore && (!source.updatedAt || source.updatedAt < ExecCount))
          runTop(source);
      } else if (state === PENDING) lookUpstream(source, ignore);
    }
  }
}
function markDownstream(node) {
  for (let i = 0; i < node.observers.length; i += 1) {
    const o = node.observers[i];
    if (!o.state) {
      o.state = PENDING;
      if (o.pure) Updates.push(o);
      else Effects.push(o);
      o.observers && markDownstream(o);
    }
  }
}
function cleanNode(node) {
  let i;
  if (node.sources) {
    while (node.sources.length) {
      const source = node.sources.pop(), index = node.sourceSlots.pop(), obs = source.observers;
      if (obs && obs.length) {
        const n = obs.pop(), s = source.observerSlots.pop();
        if (index < obs.length) {
          n.sourceSlots[s] = index;
          obs[index] = n;
          source.observerSlots[index] = s;
        }
      }
    }
  }
  if (node.owned) {
    for (i = node.owned.length - 1; i >= 0; i--) cleanNode(node.owned[i]);
    node.owned = null;
  }
  if (node.cleanups) {
    for (i = node.cleanups.length - 1; i >= 0; i--) node.cleanups[i]();
    node.cleanups = null;
  }
  node.state = 0;
}
function castError(err) {
  if (err instanceof Error) return err;
  return new Error(typeof err === "string" ? err : "Unknown error", {
    cause: err
  });
}
function handleError(err, owner = Owner) {
  const error = castError(err);
  throw error;
}
const FALLBACK = Symbol("fallback");
function dispose(d) {
  for (let i = 0; i < d.length; i++) d[i]();
}
function mapArray(list, mapFn, options = {}) {
  let items = [], mapped = [], disposers = [], len = 0, indexes = mapFn.length > 1 ? [] : null;
  onCleanup(() => dispose(disposers));
  return () => {
    let newItems = list() || [], i, j;
    newItems[$TRACK];
    return untrack(() => {
      let newLen = newItems.length, newIndices, newIndicesNext, temp, tempdisposers, tempIndexes, start, end, newEnd, item;
      if (newLen === 0) {
        if (len !== 0) {
          dispose(disposers);
          disposers = [];
          items = [];
          mapped = [];
          len = 0;
          indexes && (indexes = []);
        }
        if (options.fallback) {
          items = [FALLBACK];
          mapped[0] = createRoot((disposer) => {
            disposers[0] = disposer;
            return options.fallback();
          });
          len = 1;
        }
      } else if (len === 0) {
        mapped = new Array(newLen);
        for (j = 0; j < newLen; j++) {
          items[j] = newItems[j];
          mapped[j] = createRoot(mapper);
        }
        len = newLen;
      } else {
        temp = new Array(newLen);
        tempdisposers = new Array(newLen);
        indexes && (tempIndexes = new Array(newLen));
        for (start = 0, end = Math.min(len, newLen); start < end && items[start] === newItems[start]; start++) ;
        for (end = len - 1, newEnd = newLen - 1; end >= start && newEnd >= start && items[end] === newItems[newEnd]; end--, newEnd--) {
          temp[newEnd] = mapped[end];
          tempdisposers[newEnd] = disposers[end];
          indexes && (tempIndexes[newEnd] = indexes[end]);
        }
        newIndices = /* @__PURE__ */ new Map();
        newIndicesNext = new Array(newEnd + 1);
        for (j = newEnd; j >= start; j--) {
          item = newItems[j];
          i = newIndices.get(item);
          newIndicesNext[j] = i === void 0 ? -1 : i;
          newIndices.set(item, j);
        }
        for (i = start; i <= end; i++) {
          item = items[i];
          j = newIndices.get(item);
          if (j !== void 0 && j !== -1) {
            temp[j] = mapped[i];
            tempdisposers[j] = disposers[i];
            indexes && (tempIndexes[j] = indexes[i]);
            j = newIndicesNext[j];
            newIndices.set(item, j);
          } else disposers[i]();
        }
        for (j = start; j < newLen; j++) {
          if (j in temp) {
            mapped[j] = temp[j];
            disposers[j] = tempdisposers[j];
            if (indexes) {
              indexes[j] = tempIndexes[j];
              indexes[j](j);
            }
          } else mapped[j] = createRoot(mapper);
        }
        mapped = mapped.slice(0, len = newLen);
        items = newItems.slice(0);
      }
      return mapped;
    });
    function mapper(disposer) {
      disposers[j] = disposer;
      if (indexes) {
        const [s, set] = createSignal(j);
        indexes[j] = set;
        return mapFn(newItems[j], s);
      }
      return mapFn(newItems[j]);
    }
  };
}
let hydrationEnabled = false;
function createComponent(Comp, props) {
  if (hydrationEnabled) ;
  return untrack(() => Comp(props || {}));
}
const narrowedError = (name2) => `Stale read from <${name2}>.`;
function For(props) {
  const fallback = "fallback" in props && {
    fallback: () => props.fallback
  };
  return createMemo(mapArray(() => props.each, props.children, fallback || void 0));
}
function Show(props) {
  const keyed = props.keyed;
  const condition = createMemo(() => props.when, void 0, {
    equals: (a, b) => keyed ? a === b : !a === !b
  });
  return createMemo(
    () => {
      const c = condition();
      if (c) {
        const child = props.children;
        const fn = typeof child === "function" && child.length > 0;
        return fn ? untrack(
          () => child(
            keyed ? c : () => {
              if (!untrack(condition)) throw narrowedError("Show");
              return props.when;
            }
          )
        ) : child;
      }
      return props.fallback;
    },
    void 0,
    void 0
  );
}
function reconcileArrays(parentNode, a, b) {
  let bLength = b.length, aEnd = a.length, bEnd = bLength, aStart = 0, bStart = 0, after = a[aEnd - 1].nextSibling, map = null;
  while (aStart < aEnd || bStart < bEnd) {
    if (a[aStart] === b[bStart]) {
      aStart++;
      bStart++;
      continue;
    }
    while (a[aEnd - 1] === b[bEnd - 1]) {
      aEnd--;
      bEnd--;
    }
    if (aEnd === aStart) {
      const node = bEnd < bLength ? bStart ? b[bStart - 1].nextSibling : b[bEnd - bStart] : after;
      while (bStart < bEnd) parentNode.insertBefore(b[bStart++], node);
    } else if (bEnd === bStart) {
      while (aStart < aEnd) {
        if (!map || !map.has(a[aStart])) a[aStart].remove();
        aStart++;
      }
    } else if (a[aStart] === b[bEnd - 1] && b[bStart] === a[aEnd - 1]) {
      const node = a[--aEnd].nextSibling;
      parentNode.insertBefore(b[bStart++], a[aStart++].nextSibling);
      parentNode.insertBefore(b[--bEnd], node);
      a[aEnd] = b[bEnd];
    } else {
      if (!map) {
        map = /* @__PURE__ */ new Map();
        let i = bStart;
        while (i < bEnd) map.set(b[i], i++);
      }
      const index = map.get(a[aStart]);
      if (index != null) {
        if (bStart < index && index < bEnd) {
          let i = aStart, sequence = 1, t;
          while (++i < aEnd && i < bEnd) {
            if ((t = map.get(a[i])) == null || t !== index + sequence) break;
            sequence++;
          }
          if (sequence > index - bStart) {
            const node = a[aStart];
            while (bStart < index) parentNode.insertBefore(b[bStart++], node);
          } else parentNode.replaceChild(b[bStart++], a[aStart++]);
        } else aStart++;
      } else a[aStart++].remove();
    }
  }
}
const $$EVENTS = "_$DX_DELEGATE";
function render(code, element, init, options = {}) {
  let disposer;
  createRoot((dispose2) => {
    disposer = dispose2;
    element === document ? code() : insert(element, code(), element.firstChild ? null : void 0, init);
  }, options.owner);
  return () => {
    disposer();
    element.textContent = "";
  };
}
function template(html, isCE, isSVG) {
  let node;
  const create = () => {
    const t = document.createElement("template");
    t.innerHTML = html;
    return t.content.firstChild;
  };
  const fn = () => (node || (node = create())).cloneNode(true);
  fn.cloneNode = fn;
  return fn;
}
function delegateEvents(eventNames, document2 = window.document) {
  const e = document2[$$EVENTS] || (document2[$$EVENTS] = /* @__PURE__ */ new Set());
  for (let i = 0, l = eventNames.length; i < l; i++) {
    const name2 = eventNames[i];
    if (!e.has(name2)) {
      e.add(name2);
      document2.addEventListener(name2, eventHandler);
    }
  }
}
function setAttribute(node, name2, value) {
  if (value == null) node.removeAttribute(name2);
  else node.setAttribute(name2, value);
}
function className(node, value) {
  if (value == null) node.removeAttribute("class");
  else node.className = value;
}
function use(fn, element, arg) {
  return untrack(() => fn(element, arg));
}
function insert(parent, accessor, marker, initial) {
  if (marker !== void 0 && !initial) initial = [];
  if (typeof accessor !== "function") return insertExpression(parent, accessor, initial, marker);
  createRenderEffect((current) => insertExpression(parent, accessor(), current, marker), initial);
}
function eventHandler(e) {
  const key = `$$${e.type}`;
  let node = e.composedPath && e.composedPath()[0] || e.target;
  if (e.target !== node) {
    Object.defineProperty(e, "target", {
      configurable: true,
      value: node
    });
  }
  Object.defineProperty(e, "currentTarget", {
    configurable: true,
    get() {
      return node || document;
    }
  });
  while (node) {
    const handler = node[key];
    if (handler && !node.disabled) {
      const data = node[`${key}Data`];
      data !== void 0 ? handler.call(node, data, e) : handler.call(node, e);
      if (e.cancelBubble) return;
    }
    node = node._$host || node.parentNode || node.host;
  }
}
function insertExpression(parent, value, current, marker, unwrapArray) {
  while (typeof current === "function") current = current();
  if (value === current) return current;
  const t = typeof value, multi = marker !== void 0;
  parent = multi && current[0] && current[0].parentNode || parent;
  if (t === "string" || t === "number") {
    if (t === "number") value = value.toString();
    if (multi) {
      let node = current[0];
      if (node && node.nodeType === 3) {
        node.data !== value && (node.data = value);
      } else node = document.createTextNode(value);
      current = cleanChildren(parent, current, marker, node);
    } else {
      if (current !== "" && typeof current === "string") {
        current = parent.firstChild.data = value;
      } else current = parent.textContent = value;
    }
  } else if (value == null || t === "boolean") {
    current = cleanChildren(parent, current, marker);
  } else if (t === "function") {
    createRenderEffect(() => {
      let v = value();
      while (typeof v === "function") v = v();
      current = insertExpression(parent, v, current, marker);
    });
    return () => current;
  } else if (Array.isArray(value)) {
    const array = [];
    const currentArray = current && Array.isArray(current);
    if (normalizeIncomingArray(array, value, current, unwrapArray)) {
      createRenderEffect(() => current = insertExpression(parent, array, current, marker, true));
      return () => current;
    }
    if (array.length === 0) {
      current = cleanChildren(parent, current, marker);
      if (multi) return current;
    } else if (currentArray) {
      if (current.length === 0) {
        appendNodes(parent, array, marker);
      } else reconcileArrays(parent, current, array);
    } else {
      current && cleanChildren(parent);
      appendNodes(parent, array);
    }
    current = array;
  } else if (value.nodeType) {
    if (Array.isArray(current)) {
      if (multi) return current = cleanChildren(parent, current, marker, value);
      cleanChildren(parent, current, null, value);
    } else if (current == null || current === "" || !parent.firstChild) {
      parent.appendChild(value);
    } else parent.replaceChild(value, parent.firstChild);
    current = value;
  } else ;
  return current;
}
function normalizeIncomingArray(normalized, array, current, unwrap) {
  let dynamic = false;
  for (let i = 0, len = array.length; i < len; i++) {
    let item = array[i], prev = current && current[normalized.length], t;
    if (item == null || item === true || item === false) ;
    else if ((t = typeof item) === "object" && item.nodeType) {
      normalized.push(item);
    } else if (Array.isArray(item)) {
      dynamic = normalizeIncomingArray(normalized, item, prev) || dynamic;
    } else if (t === "function") {
      if (unwrap) {
        while (typeof item === "function") item = item();
        dynamic = normalizeIncomingArray(
          normalized,
          Array.isArray(item) ? item : [item],
          Array.isArray(prev) ? prev : [prev]
        ) || dynamic;
      } else {
        normalized.push(item);
        dynamic = true;
      }
    } else {
      const value = String(item);
      if (prev && prev.nodeType === 3 && prev.data === value) normalized.push(prev);
      else normalized.push(document.createTextNode(value));
    }
  }
  return dynamic;
}
function appendNodes(parent, array, marker = null) {
  for (let i = 0, len = array.length; i < len; i++) parent.insertBefore(array[i], marker);
}
function cleanChildren(parent, current, marker, replacement) {
  if (marker === void 0) return parent.textContent = "";
  const node = replacement || document.createTextNode("");
  if (current.length) {
    let inserted = false;
    for (let i = current.length - 1; i >= 0; i--) {
      const el = current[i];
      if (node !== el) {
        const isParent = el.parentNode === parent;
        if (!inserted && !i)
          isParent ? parent.replaceChild(node, el) : parent.insertBefore(node, marker);
        else isParent && el.remove();
      } else inserted = true;
    }
  } else parent.insertBefore(node, marker);
  return [node];
}
function arrayify(value) {
  return Array.isArray(value) ? value : [value];
}
function wrapFirstWord(word, string) {
  if (word === "") {
    return string;
  }
  const index = string.toLowerCase().indexOf(word.toLowerCase());
  if (index !== -1) {
    const wrappedWord = `<u>${string.slice(index, index + word.length)}</u>`;
    return string.slice(0, index) + wrappedWord + string.slice(index + word.length);
  } else {
    return string;
  }
}
function filter(data) {
  return data.filter((i) => i.value !== "");
}
var _tmpl$ = /* @__PURE__ */ template(`<div>`), _tmpl$2 = /* @__PURE__ */ template(`<div class="d-flex align-items-center gap-1 flex-wrap flex-grow-1 w-100 overflow-hidden">`), _tmpl$3 = /* @__PURE__ */ template(`<span role=button class=d-inline-flex>`), _tmpl$4 = /* @__PURE__ */ template(`<div class="align-items-center gap-1 d-inline-flex py-0 border-0 btn text-bg-secondary">`), _tmpl$5 = /* @__PURE__ */ template(`<div class=input-wrapper><span></span><input type=text>`), _tmpl$6 = /* @__PURE__ */ template(`<span role=button class="ms-auto link-secondary d-inline-flex flex-shrink-0">`), _tmpl$7 = /* @__PURE__ */ template(`<div class=dropdown-item>`), _tmpl$8 = /* @__PURE__ */ template(`<div class="dropdown-item active">`), _tmpl$9 = /* @__PURE__ */ template(`<div class="dropdown-item pe-none">`), _tmpl$10 = /* @__PURE__ */ template(`<div data-bs-popper class="dropdown-menu w-100">`), _tmpl$11 = /* @__PURE__ */ template(`<h6 class=dropdown-header>`), _tmpl$12 = /* @__PURE__ */ template(`<svg class=pe-none xmlns=http://www.w3.org/2000/svg width=1em height=1em viewBox="0 0 24 24"fill=none stroke=currentColor stroke-width=2 stroke-linecap=round stroke-linejoin=round><line x1=18 y1=6 x2=6 y2=18></line><line x1=6 y1=6 x2=18 y2=18>`);
const name = "use-bootstrap-select";
const classFlex = "d-flex align-items-center gap-1";
const classTarget = `${name}-target`;
const classWrapper = `${name}-wrapper`;
const eventShow = `${name}:show`;
const eventHide = `${name}:hide`;
const eventToggle = `${name}:toggle`;
const defaultConfig = {
  position: "down",
  maxHeight: "25rem",
  clearable: false,
  searchable: false,
  noResultsText: "No results found",
  creatable: false,
  creatableText: "Add <b>{value}</b>...",
  createPosition: "first"
};
const _UseBootstrapSelect = class _UseBootstrapSelect {
  constructor(selectElement, config) {
    __publicField(this, "selectElement");
    __publicField(this, "configObject");
    this.selectElement = selectElement;
    this.configObject = config;
    this.init();
    _UseBootstrapSelect.instances.set(selectElement, this);
  }
  getConfigFromGlobal() {
    return window.UseBootstrapSelectConfig;
  }
  getConfigFromAttributes() {
    const data = {};
    for (const [key, defaultValue] of Object.entries(defaultConfig)) {
      const attribute = this.selectElement.dataset[key];
      const boolean = typeof defaultValue === "boolean";
      if (attribute) {
        data[key] = boolean ? attribute === "true" : attribute;
      }
    }
    return data;
  }
  getConfig() {
    const globalConfig = this.getConfigFromGlobal();
    const configObject = this.configObject;
    const attrConfig = this.getConfigFromAttributes();
    return {
      ...defaultConfig,
      ...globalConfig,
      ...configObject,
      ...attrConfig
    };
  }
  setSelected(values, selected, clear = true) {
    const items = new Set(values);
    const options = this.selectElement.options;
    for (let i = 0; i < options.length; i++) {
      const option = options[i];
      if (selected && clear !== false) {
        option.selected = false;
      }
      if (items.has(option.value)) {
        option.selected = selected;
      }
    }
    this.update();
  }
  getValue() {
    if (this.selectElement.multiple) {
      const values = filter(this.getSelected()).map((i) => i.value);
      return values.length === 0 ? null : values;
    } else {
      const value = this.selectElement.value;
      return value === "" ? null : value;
    }
  }
  setValue(value) {
    const values = arrayify(value);
    if (!this.selectElement.multiple) {
      values.splice(1);
    }
    this.setSelected(values, true);
  }
  addValue(value) {
    const values = arrayify(value);
    this.setSelected(values, true, false);
  }
  removeValue(value) {
    const values = arrayify(value);
    this.setSelected(values, false);
  }
  clearValue() {
    this.setSelected(this.getSelected().map((i) => i.value), false);
  }
  addOption(value, text, selected = false, position = "first") {
    const label = text ?? value;
    const exist = Array.from(this.selectElement.options).some((i) => i.value === value);
    if (!exist) {
      const option = document.createElement("option");
      option.value = value;
      option.textContent = label;
      option.selected = selected;
      const options = this.selectElement.options;
      if (options.length > 0) {
        if (position === "last") {
          this.selectElement.append(option);
        } else {
          const firstOption = options[0];
          if (firstOption.value === "") {
            firstOption.insertAdjacentElement("afterend", option);
          } else {
            this.selectElement.prepend(option);
          }
        }
      } else {
        this.selectElement.append(option);
      }
      this.update();
    }
  }
  update() {
    this.selectElement.dispatchEvent(new Event("change"));
  }
  show() {
    this.selectElement.dispatchEvent(new Event(eventShow));
  }
  hide() {
    this.selectElement.dispatchEvent(new Event(eventHide));
  }
  toggle() {
    this.selectElement.dispatchEvent(new Event(eventToggle));
  }
  init() {
    if (!_UseBootstrapSelect.instances.has(this.selectElement)) {
      this.render();
      _UseBootstrapSelect.instances.set(this.selectElement, this);
    }
  }
  getClassList() {
    return this.selectElement.classList.value.replace(classTarget, "");
  }
  getItems() {
    return filter(Array.from(this.selectElement.options)).map((option) => {
      var _a;
      return {
        text: option.textContent,
        value: option.value,
        selected: option.selected,
        disabled: option.disabled,
        label: ((_a = option.parentElement) == null ? void 0 : _a.tagName) === "OPTGROUP" ? option.parentElement.label : void 0
      };
    });
  }
  getSelected() {
    return Array.from(this.selectElement.selectedOptions).map((option) => ({
      value: option.value,
      text: option.textContent
    }));
  }
  render() {
    this.selectElement.tabIndex = -1;
    this.selectElement.classList.add(classTarget);
    const wrapper = document.createElement("div");
    wrapper.className = `${classWrapper} w-100`;
    this.selectElement.insertAdjacentElement("afterend", wrapper);
    const Component = () => {
      const [config, setConfig] = createSignal(this.getConfig());
      const [classList, setClassList] = createSignal(this.getClassList());
      const [isMultiple, setIsMultiple] = createSignal(this.selectElement.multiple);
      const [isDisabled, setIsDisabled] = createSignal(this.selectElement.disabled);
      const [text, setText] = createSignal("");
      const [items, setItems] = createSignal(this.getItems());
      const [selected, setSelected] = createSignal(this.getSelected());
      const filteredItems = () => {
        items();
        return this.getItems().filter((i) => i.text.toLowerCase().includes(text().toLowerCase())).filter((i) => isMultiple() ? !selected().map((o) => o.value).includes(i.value) : true).map((option) => {
          option.text = wrapFirstWord(text(), option.text);
          return option;
        });
      };
      let dropdownMenu;
      let dropdownItemsWrapper;
      let inputElement;
      this.selectElement.addEventListener("change", () => {
        setConfig(this.getConfig());
        setClassList(this.getClassList());
        setIsMultiple(this.selectElement.multiple);
        setIsDisabled(this.selectElement.disabled);
        setItems(this.getItems());
        setSelected(this.getSelected());
      });
      this.selectElement.addEventListener("focus", () => {
        inputElement.focus();
      });
      const [focus, setFocus] = createSignal(false);
      const [shown, setShown] = createSignal(false);
      const hasPlaceholder = () => this.selectElement.options.length > 0 && this.selectElement.options[0].value === "";
      const placeholder = () => hasPlaceholder() && filter(selected()).length < 1 ? this.selectElement.options[0].text : "";
      function toggle() {
        setShown(!shown());
      }
      function show() {
        setShown(true);
      }
      function hide() {
        setShown(false);
      }
      function clearActive() {
        dropdownItemsWrapper.querySelectorAll(".dropdown-item").forEach((item) => item.classList.remove("active"));
      }
      function isIgnoredItem(element) {
        return element.classList.contains("dropdown-header") || element.classList.contains("disabled");
      }
      function findNextItem(from, direction) {
        let nextItem = direction === "ArrowUp" ? from.previousElementSibling : from.nextElementSibling;
        while (nextItem && isIgnoredItem(nextItem)) {
          nextItem = direction === "ArrowUp" ? nextItem.previousElementSibling : nextItem.nextElementSibling;
        }
        return nextItem;
      }
      function findNextDropdownItem(direction) {
        const activeItem = dropdownItemsWrapper.querySelector(".dropdown-item.active");
        let nextItem;
        if (activeItem) {
          nextItem = findNextItem(activeItem, direction);
        } else {
          const dropdownItems = dropdownItemsWrapper.querySelectorAll(".dropdown-item");
          const firstItem = direction === "ArrowDown" ? dropdownItems[0] : dropdownItems[dropdownItems.length - 1];
          if (firstItem) {
            if (isIgnoredItem(firstItem)) {
              nextItem = findNextItem(firstItem, direction);
            } else {
              nextItem = firstItem;
            }
          }
        }
        return nextItem;
      }
      const create = () => {
        this.addOption(text(), text(), true, config().createPosition);
        setText("");
        if (!isMultiple()) {
          hide();
        }
      };
      this.selectElement.addEventListener(eventShow, () => {
        show();
        inputElement.focus();
      });
      this.selectElement.addEventListener(eventHide, hide);
      this.selectElement.addEventListener(eventToggle, () => {
        if (!shown()) {
          show();
          inputElement.focus();
        } else {
          hide();
        }
      });
      const Dropdown = ({
        children
      }) => {
        return (() => {
          var _el$ = _tmpl$();
          _el$.$$mousedown = (e) => e.preventDefault();
          insert(_el$, children);
          createRenderEffect(() => className(_el$, `drop${config().position} ${name}`));
          return _el$;
        })();
      };
      const DropdownToggle = ({
        children
      }) => {
        return (() => {
          var _el$2 = _tmpl$();
          _el$2.$$mousedown = (e) => {
            if (!isDisabled() && e.target.getAttribute("role") !== "button") {
              toggle();
            }
            inputElement.focus();
          };
          insert(_el$2, children);
          createRenderEffect((_p$) => {
            var _v$ = `${classFlex} ${classList()}`, _v$2 = !!focus(), _v$3 = !!isDisabled();
            _v$ !== _p$.e && className(_el$2, _p$.e = _v$);
            _v$2 !== _p$.t && _el$2.classList.toggle("focus", _p$.t = _v$2);
            _v$3 !== _p$.a && _el$2.classList.toggle("disabled", _p$.a = _v$3);
            return _p$;
          }, {
            e: void 0,
            t: void 0,
            a: void 0
          });
          return _el$2;
        })();
      };
      const DropdownToggleContentWrapper = ({
        children
      }) => {
        return (() => {
          var _el$3 = _tmpl$2();
          insert(_el$3, children);
          return _el$3;
        })();
      };
      const DropdownToggleContentSingle = () => {
        return createMemo(() => {
          var _a;
          return createMemo(() => {
            var _a2;
            return ((_a2 = selected()[0]) == null ? void 0 : _a2.value) === "";
          })() ? "" : (_a = selected()[0]) == null ? void 0 : _a.text;
        });
      };
      const DropdownToggleContentMultiple = () => {
        const _self$ = this;
        return createComponent(For, {
          get each() {
            return filter(selected());
          },
          children: (selected2) => (() => {
            var _el$4 = _tmpl$4();
            insert(_el$4, () => selected2.text, null);
            insert(_el$4, createComponent(Show, {
              get when() {
                return !isDisabled();
              },
              get children() {
                var _el$5 = _tmpl$3();
                _el$5.$$click = (e) => {
                  e.stopPropagation();
                  _self$.removeValue(selected2.value);
                };
                insert(_el$5, createComponent(XIcon, {}));
                return _el$5;
              }
            }), null);
            createRenderEffect((_p$) => {
              var _v$4 = !!isDisabled(), _v$5 = !!classList().includes("form-select-sm"), _v$6 = !!classList().includes("form-select-lg");
              _v$4 !== _p$.e && _el$4.classList.toggle("disabled", _p$.e = _v$4);
              _v$5 !== _p$.t && _el$4.classList.toggle("btn-sm", _p$.t = _v$5);
              _v$6 !== _p$.a && _el$4.classList.toggle("btn-lg", _p$.a = _v$6);
              return _p$;
            }, {
              e: void 0,
              t: void 0,
              a: void 0
            });
            return _el$4;
          })()
        });
      };
      const InputWrapper = () => {
        const _self$2 = this;
        return (() => {
          var _el$6 = _tmpl$5(), _el$7 = _el$6.firstChild, _el$8 = _el$7.nextSibling;
          insert(_el$7, () => text() || "i");
          _el$8.$$input = (e) => {
            setText(e.target.value.trim());
            if (!shown()) {
              show();
            }
          };
          _el$8.$$keydown = (e) => {
            switch (e.key) {
              case "Enter":
                if (shown()) {
                  const activeItem = dropdownItemsWrapper.querySelector(".dropdown-item.active");
                  if (activeItem) {
                    const value = activeItem.dataset.value;
                    if (isMultiple()) {
                      _self$2.addValue(value);
                      setText("");
                    } else {
                      _self$2.setValue(value);
                      hide();
                    }
                  } else if (config().creatable) {
                    create();
                  }
                } else {
                  show();
                }
                e.preventDefault();
                break;
              case " ":
                if (!config().searchable) {
                  show();
                  e.preventDefault();
                }
                break;
              case "Escape":
                hide();
                break;
              case "ArrowUp":
              case "ArrowDown":
                !shown() && show();
                e.preventDefault();
                if (shown()) {
                  const targetItem = findNextDropdownItem(e.key);
                  if (targetItem) {
                    clearActive();
                    targetItem.classList.add("active");
                    targetItem.scrollIntoView({
                      block: "nearest"
                    });
                  }
                }
                break;
              case "Backspace":
                if (config().searchable && text() === "") {
                  if (isMultiple()) {
                    const selectedOption = selected();
                    if (selectedOption.length > 0) {
                      _self$2.removeValue(selectedOption[selectedOption.length - 1].value);
                    }
                  } else {
                    _self$2.clearValue();
                  }
                }
                break;
            }
          };
          _el$8.addEventListener("blur", () => {
            setFocus(false);
            hide();
          });
          _el$8.addEventListener("focus", () => {
            setFocus(true);
            if (config().searchable) {
              show();
            }
          });
          var _ref$ = inputElement;
          typeof _ref$ === "function" ? use(_ref$, _el$8) : inputElement = _el$8;
          createRenderEffect((_p$) => {
            var _v$7 = !!(config().searchable && focus() || filter(selected()).length < 1), _v$8 = placeholder(), _v$9 = !config().searchable && !isDisabled(), _v$10 = isDisabled();
            _v$7 !== _p$.e && _el$6.classList.toggle("position-relative", _p$.e = _v$7);
            _v$8 !== _p$.t && setAttribute(_el$8, "placeholder", _p$.t = _v$8);
            _v$9 !== _p$.a && (_el$8.readOnly = _p$.a = _v$9);
            _v$10 !== _p$.o && (_el$8.disabled = _p$.o = _v$10);
            return _p$;
          }, {
            e: void 0,
            t: void 0,
            a: void 0,
            o: void 0
          });
          createRenderEffect(() => _el$8.value = text());
          return _el$6;
        })();
      };
      const ClearButton = () => {
        const _self$3 = this;
        return createComponent(Show, {
          get when() {
            return config().clearable;
          },
          get children() {
            var _el$9 = _tmpl$6();
            _el$9.$$click = (e) => {
              e.stopPropagation();
              _self$3.clearValue();
            };
            insert(_el$9, createComponent(XIcon, {}));
            createRenderEffect(() => _el$9.classList.toggle("d-none", !!(filter(selected()).length < 1)));
            return _el$9;
          }
        });
      };
      const DropdownItemsWrapper = ({
        children
      }) => {
        return (() => {
          var _el$10 = _tmpl$();
          var _ref$2 = dropdownItemsWrapper;
          typeof _ref$2 === "function" ? use(_ref$2, _el$10) : dropdownItemsWrapper = _el$10;
          insert(_el$10, children);
          return _el$10;
        })();
      };
      const DropdownItem = ({
        item
      }) => {
        const _self$4 = this;
        return (() => {
          var _el$11 = _tmpl$7();
          _el$11.$$click = () => {
            if (isMultiple()) {
              _self$4.addValue(item.value);
              setText("");
            } else {
              _self$4.setValue(item.value);
              hide();
            }
          };
          _el$11.$$mouseover = (e) => {
            clearActive();
            e.target.classList.add("active");
          };
          createRenderEffect((_p$) => {
            var _v$11 = item.value, _v$12 = !!(item.label !== void 0), _v$13 = !!item.disabled, _v$14 = !!item.selected, _v$15 = !!item.selected, _v$16 = item.text;
            _v$11 !== _p$.e && setAttribute(_el$11, "data-value", _p$.e = _v$11);
            _v$12 !== _p$.t && _el$11.classList.toggle("optgroup-item", _p$.t = _v$12);
            _v$13 !== _p$.a && _el$11.classList.toggle("disabled", _p$.a = _v$13);
            _v$14 !== _p$.o && _el$11.classList.toggle("active", _p$.o = _v$14);
            _v$15 !== _p$.i && _el$11.classList.toggle("selected", _p$.i = _v$15);
            _v$16 !== _p$.n && (_el$11.innerHTML = _p$.n = _v$16);
            return _p$;
          }, {
            e: void 0,
            t: void 0,
            a: void 0,
            o: void 0,
            i: void 0,
            n: void 0
          });
          return _el$11;
        })();
      };
      const DropdownMenu = () => {
        return (() => {
          var _el$12 = _tmpl$10();
          var _ref$3 = dropdownMenu;
          typeof _ref$3 === "function" ? use(_ref$3, _el$12) : dropdownMenu = _el$12;
          insert(_el$12, createComponent(DropdownItemsWrapper, {
            get children() {
              return createComponent(For, {
                get each() {
                  return filteredItems();
                },
                children: (item, i) => [createComponent(Show, {
                  get when() {
                    var _a;
                    return createMemo(() => !!item.label)() && (item.label !== ((_a = filteredItems()[i() - 1]) == null ? void 0 : _a.label) || i() === 0);
                  },
                  get children() {
                    var _el$15 = _tmpl$11();
                    insert(_el$15, () => item.label);
                    return _el$15;
                  }
                }), createComponent(DropdownItem, {
                  item
                })]
              });
            }
          }), null);
          insert(_el$12, createComponent(Show, {
            get when() {
              return createMemo(() => !!(config().creatable && config().searchable && filteredItems().length < 1))() && text() !== "";
            },
            get children() {
              var _el$13 = _tmpl$8();
              _el$13.$$click = create;
              createRenderEffect(() => _el$13.innerHTML = config().creatableText.replace("{value}", text()));
              return _el$13;
            }
          }), null);
          insert(_el$12, createComponent(Show, {
            get when() {
              return createMemo(() => !!config().searchable)() && filteredItems().length < 1;
            },
            get children() {
              var _el$14 = _tmpl$9();
              insert(_el$14, () => config().noResultsText);
              return _el$14;
            }
          }), null);
          createRenderEffect((_p$) => {
            var _v$17 = !!shown(), _v$18 = config().maxHeight;
            _v$17 !== _p$.e && _el$12.classList.toggle("show", _p$.e = _v$17);
            _v$18 !== _p$.t && ((_p$.t = _v$18) != null ? _el$12.style.setProperty("max-height", _v$18) : _el$12.style.removeProperty("max-height"));
            return _p$;
          }, {
            e: void 0,
            t: void 0
          });
          return _el$12;
        })();
      };
      createEffect(() => {
        var _a;
        if (!shown()) {
          setText("");
          clearActive();
          (_a = dropdownItemsWrapper.querySelector(".dropdown-item.selected")) == null ? void 0 : _a.classList.add("active");
        }
      });
      return createComponent(Dropdown, {
        get children() {
          return [createComponent(DropdownToggle, {
            get children() {
              return [createComponent(DropdownToggleContentWrapper, {
                get children() {
                  return [createComponent(Show, {
                    get when() {
                      return !isMultiple();
                    },
                    get children() {
                      return createComponent(DropdownToggleContentSingle, {});
                    }
                  }), createComponent(Show, {
                    get when() {
                      return isMultiple();
                    },
                    get children() {
                      return createComponent(DropdownToggleContentMultiple, {});
                    }
                  }), createComponent(InputWrapper, {})];
                }
              }), createComponent(ClearButton, {})];
            }
          }), createComponent(DropdownMenu, {})];
        }
      });
    };
    render(() => createComponent(Component, {}), wrapper);
  }
  static getOrCreateInstance(selectElement) {
    let instance = _UseBootstrapSelect.instances.get(selectElement);
    if (!instance) {
      instance = new _UseBootstrapSelect(selectElement);
    }
    return instance;
  }
  static clearAll(scope) {
    (scope || document).querySelectorAll("select").forEach((selectElement) => {
      const instance = _UseBootstrapSelect.instances.get(selectElement);
      if (instance) {
        instance.clearValue();
      }
    });
  }
};
__publicField(_UseBootstrapSelect, "instances", /* @__PURE__ */ new Map());
let UseBootstrapSelect = _UseBootstrapSelect;
function XIcon() {
  return _tmpl$12();
}
delegateEvents(["mousedown", "click", "keydown", "input", "mouseover"]);
export {
  UseBootstrapSelect as default
};

export const set_storage=(key,value)=>{
  localStorage.setItem(key,JSON.stringify(value))
}

export const get_storage=(key)=>{
  return JSON.parse(localStorage.getItem(key))
}

export const remove_storage=(key)=>{
  localStorage.removeItem(key)
}

export const clear_storage=()=>{
  localStorage.clear()
}

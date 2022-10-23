import {Message,MessageBox,Notification} from 'element-ui'

export const msg_enum={
  info:'info',
  error:'error',
  success:'success',
  warning:'warning',
  danger:'danger',
  primary:'primary'
}


export function open_notification(type,title,message,duration=2000) {
  Notification[type]({
    title: title || 'Tips',
    message,
    duration: duration || 2000,
    position:'bottom-right',
    offset:10
  })
}

export function show_confirm(title,content,callback) {
  MessageBox.confirm(content, title, {
    confirmButtonText: '✔',
    cancelButtonText: 'X',
    type: msg_enum.primary
  }).then(() => {
    callback && callback();
  }).catch(()=>{});
}

export function show_delete(title,content,callback) {
  MessageBox.confirm(content, title, {
    confirmButtonText: '✔',
    cancelButtonText: 'X',
    type: msg_enum.danger
  }).then(() => {
    callback && callback();
  }).catch(()=>{});
}

// export function show_message(type,content,callback) {
//   Message[type]({
//     message: content,
//     onClose: callback,
//     duration:10000
//   })
// }

export function show_message(type,content,callback) {
  Notification[type]({
    position:"bottom-right",
    duration:5000,
    message: content,
    onClose: callback
  })
}

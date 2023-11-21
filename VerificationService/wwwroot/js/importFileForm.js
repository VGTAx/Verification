
document.addEventListener("DOMContentLoaded", () => {
   let form = document.querySelector("#importVerification");
   form.addEventListener("submit", event => {
      event.preventDefault();
      //получаем имя контроллера
      let controller = form.dataset.controller;      
      let formData = new FormData(form); 
      // ссылка запроса к методу ImportFromExcel
      let url = "/" + controller + '/ImportFromExcel'     
      
      fetch(url, {
      method: "POST",
      body: formData
      }).then(response => {
            //при успешном запросе переходим главную страницу
         if (response.ok) {
            location = "/" + controller + `/Index/`
         }         
         //если были переданы некорректные данные - обрабатываем ошибку и выводим текст в форму
         if (response.status == 400) {
            return response.json().then(data => {
               let spanAll = form.querySelectorAll("span");
               spanAll.forEach(c => c.textContent = "");
               if (data) {
                  for (let key in data) {
                     let textError = data[key].join(", ");
                     document.querySelector("#" + `${key}` + "Span").textContent = textError;
                  }
               }
            })
         }
      })
   })
})

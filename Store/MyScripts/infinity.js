class InfinityScroll{
    constructor(target_element, url, callback, amount, post_data = {}){
        this.last_view = 0
        this.count = 0
        this.amount = amount
        this.observer = new IntersectionObserver((entries) => {
            let actual = new Date()
            let diff = 0
            if (this.last_view == null) {
                diff = 1000
            } else {
                diff = (actual - this.last_view)
            }
            this.last_view = actual;
            console.log("see")
            if (entries[0].isIntersecting === true && diff >= 1000) {
                
                this.observer.unobserve(document.querySelector(`#${target_element}`))
                this.ServerCall()
            }
        }, { threshold: [0] });
        this.url = url
        this.callback = callback
        this.target_element = target_element
        this.observer.observe(document.querySelector(`#${target_element}`))
        this.post_data = post_data
        this.element_display = $(`#${target_element}`).css("display")
    }


    GenerateElements(response) {
        let data = JSON.parse(response)
        console.log(this)
        let target = $(`#${this.target_element}`)
        for (let d of data) {
            this.count++
            target.before(this.callback(d))
        }
        if (data.length > 0) {
            this.observer.observe(document.querySelector(`#${this.target_element}`))
            $(`#${this.target_element}`).css("display", "none")
        }
    }


    ServerCall() {
        const generator = this.GenerateElements.bind(this)
        $(`#${this.target_element}`).css("display", this.element_display)
        $.ajax({
            url: this.url,
            dataType: "JSON",
            method: "POST",
            data: { count: this.count, amount: this.amount, ...this.post_data },
            success: function (response) {
                $(`#${this.target_element}`).css("display", "none")               
                generator(response)

            },
        })
    }


}
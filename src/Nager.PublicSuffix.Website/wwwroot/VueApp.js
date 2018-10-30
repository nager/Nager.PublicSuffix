var app = new Vue({
    el: '#app',
    data: {
        info: null,
        domain: 'example.com'
    },
    methods: {
        analyze() {
            axios.get(`api/publicsuffix?domain=${this.domain}`)
                .then(response => {
                    // JSON responses are automatically parsed.
                    this.info = response.data
                })
                .catch(exception => {
                    console.log(exception);
                })
        }
    }
})
var app = new Vue({
    el: '#app',
    data: {
        info: null,
        domain: 'mail.example.com'
    },
    created() {
        this.analyze();
    },
    methods: {
        analyze() {
            axios.get(`api/publicsuffix?domain=${this.domain}`)
                .then(response => {
                    // JSON responses are automatically parsed.
                    this.info = response.data;
                })
                .catch(exception => {
                    console.log(exception);
                })
        }
    }
});
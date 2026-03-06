import adapter from '@sveltejs/adapter-netlify';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({
			edge: false,       // use standard Node.js functions
			split: false       // single function for all routes
		})
	}
};

export default config;

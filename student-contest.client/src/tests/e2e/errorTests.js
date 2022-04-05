import { Selector, RequestMock } from 'testcafe';
import { login } from './auth';
import configData from '../../config.json';

const url = configData.SERVER_URL + '/login';
const mock404 = RequestMock().onRequestTo(url).respond(null, 404);
const mock500 = RequestMock().onRequestTo(url).respond(null, 500);

fixture`Main page`.page`http://localhost:3000`;

test('navigate to non-existing page redirect to error page', async (t) => {
	await t
		.navigateTo('../smthStupid')
		.expect(Selector('p').innerText)
		.contains('removed')
		.expect(Selector('a.not-found').innerText)
		.eql('Take me back to the homepage');
});

test.requestHooks(mock500)(
	'server respond with error other than 404 redirect to error page',
	async (t) => {
		await login(t);
		await t
			.expect(Selector('p').innerText)
			.contains('try again')
			.expect(Selector('a.not-found').innerText)
			.eql('Take me back to the homepage');
	}
);

test.requestHooks(mock404)(
	'server respond with 404 redirect to not found page',
	async (t) => {
		await login(t);
		await t
			.expect(Selector('p').innerText)
			.contains('removed')
			.expect(Selector('a.not-found').innerText)
			.eql('Take me back to the homepage');
	}
);

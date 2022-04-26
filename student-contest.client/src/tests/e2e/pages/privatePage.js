import { Selector, t } from 'testcafe';

const PrivatePage = () => {
	const infoMsg = Selector('p');

	return {
		async assertInfoMsgText() {
			await t.expect(infoMsg.innerText).eql('Some info');
		}
	};
};

export default PrivatePage();

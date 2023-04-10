$(function () {
    $(".btnCheckSshHostKeyFingerprint").click(function (e) {
        //alert(789);
        var valid = true;
        var Host = $(".Host").val();
        if (Host == null || Host == "") {
            alert("Host can't be empty or blank.!");
            return;
        }
        var Port = $(".Port").val();
        if (Port == null || Port == "") {
            alert("Port can't be empty or blank.!");
            return;
        }
        var UserName = $(".UserName").val();
        if (UserName == null || UserName == "") {
            alert("User Name can't be empty or blank.!");
            return;
        }
        var postedFile = document.getElementById('postedFile');
        if (postedFile.files.length == 0 || postedFile.files[0] == null) {
            alert("Please select private key file.!");
            return;
        }
        if (valid) {
            debugger;
            var url = "/ValidateFtp/GetSshHostKeyFingerprint";
            const formData = new FormData();
            let file = $('#postedFile').prop("files")[0];
            formData.append('file', file);
            formData.append('Host', Host);
            formData.append('Port', Port);
            formData.append('UserName', UserName);
            formData.append('SshHostKeyFingerprint', $(".SshHostKeyFingerprint").val());
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: (result) => {
                    $(".SshHostKeyFingerprint").val(result.sshHostKeyFingerprint);
                    SshHostKeyFingerprintChange();
                }
            });
        }
    });
    $(".Host").change(function () {
        $(".SshHostKeyFingerprint").val("");
        $(".btnCheckSshHostKeyFingerprint").css("display", "block");
        $(".btnValidate").css("display", "none");
    });
    $(".Port").change(function () {
        $(".SshHostKeyFingerprint").val("");
        $(".btnCheckSshHostKeyFingerprint").css("display", "block");
        $(".btnValidate").css("display", "none");
    });
    $(".UserName").change(function () {
        $(".SshHostKeyFingerprint").val("");
        $(".btnCheckSshHostKeyFingerprint").css("display", "block");
        $(".btnValidate").css("display", "none");
    });
    $("#postedFile").change(function () {
        $(".SshHostKeyFingerprint").val("");
        $(".btnCheckSshHostKeyFingerprint").css("display", "block");
        $(".btnValidate").css("display", "none");
    });
    $(".SshHostKeyFingerprint").change(function () {
        if ($(".SshHostKeyFingerprint").val() == "" || $(".SshHostKeyFingerprint").val() == null) {
            $(".SshHostKeyFingerprint").val("");
            $(".btnCheckSshHostKeyFingerprint").css("display", "block");
            $(".btnValidate").css("display", "none");
        }
        else {
            $(".btnCheckSshHostKeyFingerprint").css("display", "none");
            $(".btnValidate").css("display", "block");
        }
    });
    SshHostKeyFingerprintChange = function () {
        if ($(".SshHostKeyFingerprint").val() == "" || $(".SshHostKeyFingerprint").val() == null) {
            $(".SshHostKeyFingerprint").val("");
            $(".btnCheckSshHostKeyFingerprint").css("display", "block");
            $(".btnValidate").css("display", "none");
        }
        else {
            $(".btnCheckSshHostKeyFingerprint").css("display", "none");
            $(".btnValidate").css("display", "block");
        }
    }
    var postedFile = document.getElementById('postedFile');
    if ($(".Host").val() == null || $(".Host").val() == "" || $(".Port").val() == null || $(".Port").val() == "" || $(".UserName") == "" || $(".UserName") == null || postedFile.files.length == 0 || postedFile.files[0] == null) {
        $(".btnCheckSshHostKeyFingerprint").css("display", "block");
        $(".btnValidate").css("display", "none");
    }
});